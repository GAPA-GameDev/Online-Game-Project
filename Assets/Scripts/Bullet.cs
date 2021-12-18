using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitEffect;
    public PlaceHolderMOVEMENT player;
    public Player2 player2;

    void OnCollisionEnter(Collision other)
    {

        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);

        if(other.gameObject.tag == "Player") //When enemy bullet hits player (Not online)
        {
            player.ReceiveDamage();
        }

        if (other.gameObject.tag == "Player2") //If bullet hits enemy player (This should only happen when allied bullet hits other player)
        {
            player.client.OnHit();
            player2.ReceiveBulletDamage();
        }

        Destroy(effect, 2.0f);
        Destroy(gameObject);
    }
}
