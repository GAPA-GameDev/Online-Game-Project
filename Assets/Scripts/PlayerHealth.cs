using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    public GameManager gameManager;

    public int playernum = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playernum == 1)
        {
            gameObject.GetComponent<Text>().text = string.Concat("Player 1 Health: ", gameManager.player1Script.health.ToString());
        }
        else
        {
            gameObject.GetComponent<Text>().text = string.Concat("Player 2 Health: ", gameManager.player2Script.health.ToString());
        }
    }
}
