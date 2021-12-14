using UnityEngine;

using System.Text;

using System.Net;
using System.Net.Sockets;

using UnityEngine.UI;

enum ClientState
{
    LOGIN,
    WAITING,
    PLAYING,
    NONE
}

public class Peer2PeerClient : MonoBehaviour
{
    ClientState state = ClientState.LOGIN;

    private UdpClient socket;

    public string clientName;

    public int port;
    public int otherPort;
    string host = "127.0.0.1";

    public InputField portInput;
    public InputField opponentPortInput;

    public GameObject loginMenu;

    IPEndPoint otherClientIP;

    bool received = true;

    //Possibly have an array of messages to send (All actions the player wants the other one to know)


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        switch(state)
        {
            
            case ClientState.LOGIN:



                break;

            case ClientState.WAITING:


                if (socket.Client.Poll(0, SelectMode.SelectRead))
                {

                    byte[] buffer = socket.Receive(ref otherClientIP);
                    Debug.Log(string.Concat("Client ", clientName, " received: ", Encoding.ASCII.GetString(buffer)));


                    string str1 = "Hello";
                    byte[] buffer1;
                    buffer1 = Encoding.ASCII.GetBytes(str1);
                    socket.Send(buffer1, buffer1.Length, host, otherPort);
                    Debug.Log(string.Concat("Client ", clientName, " sent: ", str1));
                    

                    //Start Game
                    state = ClientState.PLAYING;


                }

                break;

            case ClientState.PLAYING:

                if (received)
                {
                    string str = "Fuck";
                    byte[] buffer;
                    buffer = Encoding.ASCII.GetBytes(str);
                    socket.Send(buffer, buffer.Length, host, otherPort);
                    Debug.Log(string.Concat("Client ", clientName, " sent: ", str));
                    received = false;
                }

                if (socket.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buffer = socket.Receive(ref otherClientIP);
                    Debug.Log(string.Concat("Client ", clientName, " received: ", Encoding.ASCII.GetString(buffer)));
                    received = true;
                }

                break;
        }



    }

    public void OnLogin()
    {
        if(portInput.text.Length >0 && opponentPortInput.text.Length >0)
        {
            port = int.Parse(portInput.text);
            otherPort = int.Parse(opponentPortInput.text);

            socket = new UdpClient(port);
            otherClientIP = new IPEndPoint(IPAddress.Any, otherPort); //Here we save the other client's IP to send messages

            loginMenu.SetActive(false);

            state = ClientState.WAITING;

            try
            {
                string str1 = "Hello";
                byte[] buffer1;
                buffer1 = Encoding.ASCII.GetBytes(str1);
                socket.Send(buffer1, buffer1.Length, host, otherPort);
                Debug.Log(string.Concat("Client ", clientName, " sent: ", str1));
            }
            catch
            {
                Debug.Log(string.Concat(clientName,": Other client is not available yet"));
            }
            

            
        }
    }
}
