using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    float speed = 10;

    public Peer2PeerClient client;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(client.state)
        {
            case ClientState.PLAYING: //Simple movement script, only when it's playing

                if (Input.GetAxis("Horizontal") != 0)
                {
                    transform.localPosition += new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0, 0);
                }

                if (Input.GetAxis("Vertical") != 0)
                {
                    transform.localPosition += new Vector3(0, 0, Input.GetAxis("Vertical") * speed * Time.deltaTime);
                }

                break;
        }
        
    }
}
