using UnityEngine;

using System.Text;
using System.IO;
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
    OBJECT_STATE,
    GAME_STATE,
    CONNECTION_STATE,
    UNKNOWN
}
public class ByteConstants
{
    public byte X_POSITION_MASK = 0b_0000_0001;
    public byte Y_POSITION_MASK = 0b_0000_0010;
    public byte Z_POSITION_MASK = 0b_0000_0100;
}
class ClientInfo
{
    //non modifyable
    public string playerID;

    //modifyable
    public float x, y, z;
}
class TransformMessage
{
    public Transform newTransform;
}

class BinarySerializer
{
    //COMMAND STRUCTURE
    // - Opening character: "#"
    // - Header: packet size | player identifyer (network ID) | packet type (Object state, Game state or Connection state)
    //   maybe adding a packet identifyer to the header could be useful to check if it has arrived to the other user that would send
    //   an immediate response communicationg the packet that just arrived. May also help with redundant packets;
    //   just check and compare the packet id, and you'll know if it has already been received or not.
    // - Contents: 
    public void SerializeCommands()
    {
        byte[] resp = new byte[2048];
        var memStream = new MemoryStream();



        using (BinaryWriter writer = new BinaryWriter(memStream)) //let's use a memory stream for the moment
        {
            writer.Write("#");
            //writer.Write();
        }
    }
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

    ByteConstants constants;
    public byte changedParameters = 0b00000000;

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
                Debug.Log(string.Concat(clientName, ": Other client is not available yet"));
            }
        }
    }
}

