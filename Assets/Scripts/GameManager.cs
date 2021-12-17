using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameState
{
    NONE =0,
    START_GAME,
    SPAWNING,
    PLAYING,
    NEXT_ROUND,
    END_SCREEN
}

public class GameManager : MonoBehaviour
{

    int round = 0;
    int maxRounds = 3;

    public Peer2PeerClient client;

    GameState gameState = GameState.NONE;

    public GameObject player; //The client's player
    public GameObject enemyPlayer; //

    public GameObject pauseMenu;

    bool paused = false; //If pause menu is open, just stop player movement/input

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        switch(gameState)
        {
            case GameState.START_GAME:

                //Spawn players and scene


                break;

            case GameState.NEXT_ROUND:


                break;

            case GameState.SPAWNING:


                break;

            case GameState.PLAYING:


                break;

            case GameState.END_SCREEN:


                break;

        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            
        }

    }

    public void StartGame()
    {
        round = 0;

        gameState = GameState.START_GAME; //Start first round spawning everything anew
    }

    public void OnDisconnect()
    {
        //Maybe show player that the other one disconnected?

    }

    //PlayerNum refers to the player which will be acted upon 
    public void MovePlayer(int playerNum,Transform newTrans) //What to do when receiving movement from client
    {
        if (playerNum == 1) //Send player to the left (-x)
        {
            enemyPlayer.transform.localPosition = new Vector3(newTrans.localPosition.x - client.screenOffset, newTrans.localPosition.y, newTrans.localPosition.z);
            enemyPlayer.transform.localRotation = newTrans.localRotation;
        }
        else
        {
            enemyPlayer.transform.localPosition = new Vector3(newTrans.localPosition.x + client.screenOffset, newTrans.localPosition.y, newTrans.localPosition.z);
            enemyPlayer.transform.localRotation = newTrans.localRotation;
        }
        
    }
}
