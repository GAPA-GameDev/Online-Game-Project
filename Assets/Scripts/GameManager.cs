using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum GameState
{
    NONE =0,
    START_GAME,
    PLAYING,
    NEXT_ROUND,
    END_SCREEN
}

public class GameManager : MonoBehaviour
{



    int round = 0;
    int maxRounds = 3;

    int player1Wins = 0; //Amount of wins Player1 has
    int player2Wins = 0; //Amount of wins Player2 has

    float endRoundTimer = 0.0f;
    float nextRoundTimer = 0.0f;
    float endGameTimer = 0.0f;

    public GameObject EndRoundMenu;
    public GameObject EndGameMenu;
    public Text endRoundText;
    public Text endGameText;

    bool roundEnded = false;
    bool roundSetUp = false; //If the round has been set up yet

    public Peer2PeerClient client;

    public GameState gameState = GameState.NONE;

    public GameObject player; //The client's player
    public GameObject enemyPlayer; //

    public TextMeshPro player1Usernam;
    public TextMeshPro player2Username;

    public PlaceHolderMOVEMENT player1Script;
    public Player2 player2Script;


    public GameObject pauseMenu;

    public bool paused = false; //If pause menu is open, just stop player movement/input
    public bool allowInput = false; //Allows player to move their character

    public Transform spawnPointPlayer1;
    public Transform spawnPointPlayer2;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        EndRoundMenu.SetActive(false);
        EndGameMenu.SetActive(false);

        player.SetActive(false);
        enemyPlayer.SetActive(false);

        player1Script = player.GetComponent<PlaceHolderMOVEMENT>();
        player2Script = enemyPlayer.GetComponent<Player2>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.START_GAME:

                allowInput = false;

                //Set Up new Game
                round = 0;

                player1Usernam.text = client.username;
                player2Username.text = client.enemyUsername;

                gameState = GameState.NEXT_ROUND;

                break;

            case GameState.NEXT_ROUND:

                allowInput = false;

                if(!roundSetUp)
                {
                    SetUpNewRound();
                    roundSetUp = true;
                }

                if(nextRoundTimer >= 2)
                {
                    round++;
                    gameState = GameState.PLAYING;
                    nextRoundTimer = 0.0f;
                    roundSetUp = false;
                }
                else
                {
                    nextRoundTimer += Time.deltaTime;
                }

                break;

            case GameState.PLAYING:

                allowInput = true;
                UpdatePause();

                if(player1Script.health <= 0) //End game, player 2 wins!!
                {
                    roundEnded = true;
                    player2Wins++;
                    endRoundText.text = string.Concat("You Lost round ", round.ToString());
                }
                
                if(player2Script.health <=0) //End game, player 1 wins!!
                {
                    roundEnded = true;
                    player1Wins++;
                    endRoundText.text = string.Concat("You Won round ", round.ToString());
                }

                if (roundEnded)
                {
                    allowInput = false;

                    EndRoundMenu.SetActive(true);
                    //Set text to say who won


                    //Maybe start a timer to display the round winner
                    if (endRoundTimer >= 3)
                    {
                        endRoundTimer = 0.0f;

                        roundEnded = false;

                        EndRoundMenu.SetActive(false);

                        if (round < maxRounds) //If there are more rounds just go to the next one
                        {
                            
                            if((player1Wins > (maxRounds/2.0f)) || (player2Wins > (maxRounds / 2.0f)))
                            {
                                gameState = GameState.END_SCREEN;

                                EndGameMenu.SetActive(true);
                            }
                            else
                            {
                                gameState = GameState.NEXT_ROUND;
                            }

                        }
                        else //If ther rounds are over then the game is over!!
                        {
                            gameState = GameState.END_SCREEN;

                            EndGameMenu.SetActive(true);
                        }
                    }
                    else
                    {
                        endRoundTimer += Time.deltaTime;
                    }
                }

                break;

            case GameState.END_SCREEN:

                allowInput = false;

                //Count wins and display who won the game! (Start a timer to show the winner)

                if(player1Wins > player2Wins)
                {
                    //Player1 won!
                    endGameText.text = "You Won the Game!";
                }
                else if(player2Wins == player1Wins)
                {
                    //Draw I guess
                }
                else
                {
                    //Player2Won!!
                    endGameText.text = "You lost the Game!";
                }
                
                if(endGameTimer >= 3)
                {
                    gameState = GameState.START_GAME;
                    EndGameMenu.SetActive(false);
                    endGameTimer = 0.0f;
                    player1Wins = 0;
                    player2Wins = 0;
                }
                else
                {
                    endGameTimer += Time.deltaTime;
                }

                break;

        }

    }

    public void StartGame()
    {
        gameState = GameState.START_GAME; //Start first round spawning everything anew
    }

    void SetUpNewRound()
    {
        if(round <= maxRounds) //If less than the max rounds just spawn them with max health
        {

            //Spawn and Move players to starting positions
            player.SetActive(true);
            enemyPlayer.SetActive(true);

            if (client.playerNum == 1) //Send player to the left (-x) .-------------------- This is only because of testing with two scenes at the same time
            {
                player.transform.localPosition = new Vector3(spawnPointPlayer1.localPosition.x - client.screenOffset, spawnPointPlayer1.localPosition.y, spawnPointPlayer1.localPosition.z);
                enemyPlayer.transform.localPosition = new Vector3(spawnPointPlayer2.localPosition.x - client.screenOffset, spawnPointPlayer2.localPosition.y, spawnPointPlayer2.localPosition.z);
            }
            else
            {
                player.transform.localPosition = new Vector3(spawnPointPlayer2.localPosition.x , spawnPointPlayer2.localPosition.y, spawnPointPlayer2.localPosition.z);
                enemyPlayer.transform.localPosition = new Vector3(spawnPointPlayer1.localPosition.x , spawnPointPlayer1.localPosition.y, spawnPointPlayer1.localPosition.z);
            }
                

            player1Script.health = player1Script.maxHealth;
            player2Script.health = player2Script.maxHealth;



        }
    }

    public void OnDisconnect()
    {
        //Maybe show player that the other one disconnected?
        pauseMenu.SetActive(false);


    }

    //PlayerNum refers to the player which will be acted upon 
    public void MovePlayer(int playerNum,TransformMessage newTrans) //What to do when receiving movement from client
    {
        if(gameState == GameState.PLAYING)
        {
            if (playerNum == 2) //Send player to the left (-x)
            {
                enemyPlayer.transform.localPosition = newTrans.localPos; //new Vector3(newTrans.localPosition.x + client.screenOffset, newTrans.localPosition.y, newTrans.localPosition.z);
                enemyPlayer.transform.localRotation = newTrans.rotation;
            }
            else
            {
                enemyPlayer.transform.localPosition = new Vector3(newTrans.localPos.x - client.screenOffset, newTrans.localPos.y, newTrans.localPos.z);
                enemyPlayer.transform.localRotation = newTrans.rotation;
            }
        }
        
    }

    void UpdatePause()
    {

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                paused = false;
                pauseMenu.SetActive(false);
            }
            else
            {
                paused = true;
                pauseMenu.SetActive(true);
            }
        }
        
    }
}
