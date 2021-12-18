using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{

    public GameObject bulletPrefab; 
    public PlaceHolderMOVEMENT player;
    public GameManager gameManager;

    public float health = 100.0f;
    public float maxHealth = 100.0f;

    public float bulletDamage = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShootBullet(ShotMessage shot) //Replicates the other player's shot
    {
        GameObject bullet = Instantiate(bulletPrefab, shot.initialPos, shot.initialRotation);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.player = player;
        bulletScript.player2 = this;


        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Vector3 force = shot.initialForce;
        rb.AddForce(force, ForceMode.Impulse);
    }

    public void ReceiveBulletDamage()
    {
        if(gameManager.allowInput)
        {
            health -= 10; //Why tf can I not do it with a variable
        }
    }
}
