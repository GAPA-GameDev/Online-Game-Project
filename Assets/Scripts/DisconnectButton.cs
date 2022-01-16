using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisconnectButton : MonoBehaviour
{

    public string clientName;

    //Peer2PeerClient client;

    // Start is called before the first frame update
    void Start()
    {
        //client = GameObject.Find(clientName).GetComponent< Peer2PeerClient>();

        gameObject.GetComponent<Button>().onClick.AddListener(OnClickDisconnect);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClickDisconnect()
    {
        //client.OnDisconnect();
    }
}
