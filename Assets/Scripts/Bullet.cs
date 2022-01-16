using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Bullet : MonoBehaviour
{
    public GameObject hitEffect;
    public PlaceHolderMOVEMENT player;
    public PlaceHolderMOVEMENT player2;
    GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnCollisionEnter(Collision other)
    {
        
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);

        if(other.gameObject == gameManager.player1Script.gameObject) //When enemy bullet hits player (Not online)
        {
            gameManager.player1Script.ReceiveDamage();
            
        }

        if (other.gameObject == gameManager.player2Script.gameObject) //If bullet hits enemy player (This should only happen when allied bullet hits other player)
        {
            //player.client.OnHit();
            gameManager.player2Script.ReceiveDamage();
            PhotonNetwork.RaiseEvent(0, null, RaiseEventOptions.Default,SendOptions.SendReliable);
        }

        PhotonNetwork.Destroy(GetComponent<PhotonView>());
        Destroy(effect, 2.0f);
        Destroy(gameObject);
    }
}
