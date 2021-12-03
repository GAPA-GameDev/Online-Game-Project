using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;

using System.Net;
using System.Net.Sockets;

public class Peer2PeerClient : MonoBehaviour
{

    private UdpClient socket;

    public string clientName;

    public int port;
    public int otherPort;
    string host = "127.0.0.1";

    IPEndPoint otherClientIP;

    bool received = true;

    //Possibly have an array of messages to send (All actions the player wants the other one to know)


    // Start is called before the first frame update
    void Start()
    {
        socket = new UdpClient(port);

        otherClientIP = new IPEndPoint(IPAddress.Any, otherPort); //Here we save the other client's IP to send messages
    }

    // Update is called once per frame
    void Update()
    {

        if(received)
        {
            string str = "Fuck";
            byte[] buffer;
            buffer = Encoding.ASCII.GetBytes(str);
            socket.Send(buffer,buffer.Length, host, otherPort);
            Debug.Log(string.Concat("Client ", clientName, " sent: ", str));
            received = false;
        }

        if(socket.Client.Poll(0,SelectMode.SelectRead))
        {
            byte[] buffer = socket.Receive(ref otherClientIP);
            Debug.Log(string.Concat("Client ", clientName, " received: ", Encoding.ASCII.GetString(buffer)));
            received = true;
        }



    }
}
