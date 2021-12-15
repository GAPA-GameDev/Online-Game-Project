using UnityEngine;

using System.Text;

using System.Net;
using System.Net.Sockets;

using UnityEngine.UI;

public enum ClientState
{
    LOGIN,
    WAITING,
    PLAYING,
    NONE
}

enum MessageType
{

}

class TransformMessage
{
    public Transform newTransform;
}

public class Peer2PeerClient : MonoBehaviour
{

    public int playerNum = 1; //either 1 or 2 (This helps when having both scenes at the same screen)
    public float screenOffset = 0;

    public ClientState state = ClientState.LOGIN;

    private UdpClient socket; //This clien'ts Udp socket

    public string clientName; //This will be used later on, right now only for logs

    public int port; //This client's port (Local)
    public int otherPort; //Other clien'ts port (Local)
    string host = "127.0.0.1";

    public InputField portInput; //Input field for this client's port
    public InputField opponentPortInput; //Input field for the other client's port

    public GameObject loginMenu; //Object holding all the LoginMenu to be deactivated

    public GameObject player; //The client's player
    public GameObject opponentPlayer; //

    IPEndPoint otherClientIP;

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
                    //We receive the first message from the other client 
                    byte[] buffer = socket.Receive(ref otherClientIP);
                    Debug.Log(string.Concat("Client ", clientName, " received: ", Encoding.ASCII.GetString(buffer)));

                    //Here we send the message again because one of the clients connects later than the other one
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

                //Right now we constantly send the TransformMessage, later on we should make it so it waits for the other client to respond after getting the message to send it again?
                TransformMessage newTransform = new TransformMessage();
                newTransform.newTransform = player.transform; //Player's transform
                string str = JsonUtility.ToJson(newTransform); //Serialized with Json

                byte[] buffer2 = Encoding.ASCII.GetBytes(str); //Encoded with ASCII
                socket.Send(buffer2, buffer2.Length, host, otherPort);

                //Debug.Log(string.Concat("Client ", clientName, " sent: ", str));

                if (socket.Client.Poll(0, SelectMode.SelectRead))
                {
                    //Here I'm assuming I'm getting a TransformMessage, Later we should deserialize and see what kind of message it is
                    byte[] buffer = socket.Receive(ref otherClientIP);

                    string tmp = Encoding.ASCII.GetString(buffer); //Decode from ASCII

                    TransformMessage newTrans = JsonUtility.FromJson<TransformMessage>(tmp); //Deserialize from Json

                    if(playerNum ==1) //Send player to the left (-x)
                    {
                        opponentPlayer.transform.localPosition = new Vector3(newTrans.newTransform.localPosition.x - screenOffset, newTrans.newTransform.localPosition.y, newTrans.newTransform.localPosition.z);
                        opponentPlayer.transform.localRotation = newTrans.newTransform.localRotation;

                    }
                    else //Send Player to the right (+x)
                    {
                        opponentPlayer.transform.localPosition = new Vector3(newTrans.newTransform.localPosition.x + screenOffset, newTrans.newTransform.localPosition.y, newTrans.newTransform.localPosition.z);
                        opponentPlayer.transform.localRotation = newTrans.newTransform.localRotation;
                    }

                }

                break;
        }



    }

    public void OnLogin() //This is called when the login button is pressed
    {
        if(portInput.text.Length >0 && opponentPortInput.text.Length >0) //Only if there is something written on the login boxes (This should have more safety checks later on)
        {
            port = int.Parse(portInput.text);
            otherPort = int.Parse(opponentPortInput.text);

            socket = new UdpClient(port);
            otherClientIP = new IPEndPoint(IPAddress.Any, otherPort); //Here we save the other client's IP to send messages

            loginMenu.SetActive(false);

            state = ClientState.WAITING;

            try
            {
                //Send a first message so that the other client knows we are here and stops waiting
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
