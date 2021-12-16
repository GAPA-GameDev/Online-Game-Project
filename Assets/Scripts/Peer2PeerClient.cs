using UnityEngine;

using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

using UnityEngine.UI;

using UnityEngine.UIElements;

public enum ClientState
{
    LOGIN,
    WAITING,
    PLAYING,
    NONE
}

enum PacketType
{
    OBJECT_STATE,
    GAME_STATE,
    CONNECTION_STATE,
    UNKNOWN
}

enum MessageType
{
    CONNECT,
    PLAYER_MOVE,
    PLAYER_SHOOT,
    DISCONNECT,
    NONE = 0
}

class Message
{
    public MessageType type;
    public string message;
}
public class ByteConstants
{
    public const byte X_POSITION_MASK = 0b_0000_0001;
    public const byte Y_POSITION_MASK = 0b_0000_0010;
    public const byte Z_POSITION_MASK = 0b_0000_0100;
}
public class ObjectStateInfo
{
    //non modifyable
    public string objectID;

    //modifyable
    public float x, y, z;
    public byte changedParameters = 0b00000000;
}
class TransformMessage
{
    public Transform newTransform;
}

class BinarySerializer
{
    ByteConstants constants;

    //PACKET STRUCTURE
    // - Opening character: "#"
    // - Header: packet size | player identifyer (network ID) | packet type (Object state, Game state or Connection state)
    //   maybe adding a packet identifyer to the header could be useful to check if it has arrived to the other user that would send
    //   an immediate response communicationg the packet that just arrived. May also help with redundant packets;
    //   just check and compare the packet id, and you'll know if it has already been received or not.
    // - ObjectStateContents: 
    public void SerializeObjectState(ObjectStateInfo ourInfo)
    {
        PacketType type = PacketType.OBJECT_STATE;
        int packetSize = 0;
        int playerID = 0;

        byte[] resp = new byte[2048];
        var memStream = new MemoryStream();

        //Calculate packet size


        using (BinaryWriter writer = new BinaryWriter(memStream)) //let's use a memory stream for the moment
        {
            writer.Write("#");
            //writer.Write();
        }
    }

    // - GameStateContents: 
    public void SerializeGameState(uint playerID)
    {

    }

    // - ConnectionStateContents:
    public void SerializeConnectionState(uint playerID)
    {

    }
}

class ConnectMessage
{
    public string username;
    public Color color;
    public int playerNum = 1; //The number player the user is
}

class DisconnectMessage //Idk if this should do anything
{


}

public class Peer2PeerClient : MonoBehaviour
{

    public int playerNum = 1; //either 1 or 2 (This helps when having both scenes at the same screen and deciding the starting position)
    public float screenOffset = 0;

    public ClientState state = ClientState.LOGIN;

    private UdpClient socket; //This clien'ts Udp socket

    string username = "None";
    Color color = Color.black;

    string enemyUsername = "None";
    Color enemyColor = Color.black;

    public int port; //This client's port (Local)
    public int otherPort; //Other clien'ts port (Local)
    string host = "127.0.0.1";

    public InputField portInput; //Input field for this client's port
    public InputField opponentPortInput; //Input field for the other client's port

    public GameObject loginMenu; //Object holding all the LoginMenu to be deactivated
    public GameObject GameMenu;

    public GameObject player; //The client's player
    public GameObject enemyPlayer; //

    IPEndPoint otherClientIP;

    public ObjectStateInfo ourClientInfo;
    ByteConstants constants;

    //Possibly have an array of messages to send (All actions the player wants the other one to know)


    // Start is called before the first frame update
    void Start()
    {
        GameMenu.SetActive(false);
        loginMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        byte[] buffer;
        string stringBuffer;

        switch (state)
        {

            case ClientState.LOGIN:
                


                break;

            case ClientState.WAITING:

                if (socket.Client.Poll(0, SelectMode.SelectRead))
                {
                    buffer = socket.Receive(ref otherClientIP);
                    stringBuffer = Encoding.ASCII.GetString(buffer); //Decode from ASCII
                    Message receivedMessage = JsonUtility.FromJson<Message>(stringBuffer); //Deserialize from Json

                    switch (receivedMessage.type) //Here we have the switch for the different messages received
                    {
                        case MessageType.CONNECT:

                            //Fill in opponent's info
                            ConnectMessage connectMessage = JsonUtility.FromJson<ConnectMessage>(receivedMessage.message);

                            enemyUsername = connectMessage.username;
                            enemyColor = connectMessage.color;
                            
                            if(connectMessage.playerNum == 1)
                            {
                                playerNum = 2;
                            }
                            else
                            {
                                playerNum = 1;
                            }

                            SendMessage(MessageType.CONNECT);

                            //Start Game

                            StartGame();

                            state = ClientState.PLAYING;

                            break;
                    }
                }


                break;

            case ClientState.PLAYING:

                //Right now we constantly send the TransformMessage, later on we should make it so it waits for the other client to respond after getting the message to send it again?
                
                SendMessage(MessageType.PLAYER_MOVE);

                //Debug.Log(string.Concat("Client ", clientName, " sent: ", str));

                while (socket.Client.Poll(0, SelectMode.SelectRead))
                {

                    buffer = socket.Receive(ref otherClientIP);
                    stringBuffer = Encoding.ASCII.GetString(buffer); //Decode from ASCII
                    Message receivedMessage2 = JsonUtility.FromJson<Message>(stringBuffer); //Deserialize from Json


                    switch (receivedMessage2.type) //Here we have the switch for the different messages received
                    {
                        case MessageType.PLAYER_MOVE:

                            TransformMessage transformMessage = JsonUtility.FromJson<TransformMessage>(receivedMessage2.message);
                            Transform newTrans = transformMessage.newTransform;

                            if (playerNum == 1) //Send player to the left (-x)
                            {
                                enemyPlayer.transform.localPosition = new Vector3(newTrans.localPosition.x - screenOffset, newTrans.localPosition.y, newTrans.localPosition.z);
                                enemyPlayer.transform.localRotation = newTrans.localRotation;

                            }
                            else //Send Player to the right (+x)
                            {
                                enemyPlayer.transform.localPosition = new Vector3(newTrans.localPosition.x + screenOffset, newTrans.localPosition.y, newTrans.localPosition.z);
                                enemyPlayer.transform.localRotation = newTrans.localRotation;
                            }

                            break;

                        case MessageType.DISCONNECT:

                            Disconnect();

                            break;
                    }



                }

                break;
        }
    }

    Message SendMessage(MessageType type)
    {
        Message ret = new Message();

        switch(type)
        {
            case MessageType.CONNECT:

                ConnectMessage connectMessage = new ConnectMessage();

                connectMessage.username = username;
                connectMessage.color = color;
                connectMessage.playerNum = playerNum;

                ret.type = MessageType.CONNECT;
                ret.message = JsonUtility.ToJson(connectMessage); //Serialized with Json

                break;

            case MessageType.DISCONNECT:

                DisconnectMessage disconnectMessage = new DisconnectMessage();

                ret.type = MessageType.DISCONNECT;
                ret.message = JsonUtility.ToJson(disconnectMessage); //Serialized with Json

                break;

            case MessageType.PLAYER_MOVE:

                TransformMessage newTransform = new TransformMessage();
                newTransform.newTransform = player.transform; //Player's transform

                ret.type = MessageType.PLAYER_MOVE;
                ret.message = JsonUtility.ToJson(newTransform); //Serialized with Json

                break;
        }


        string messageString = JsonUtility.ToJson(ret);

        byte[] buffer = Encoding.ASCII.GetBytes(messageString); //Encoded with ASCII
        socket.Send(buffer, buffer.Length, host, otherPort);

        return ret;
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
            GameMenu.SetActive(true);

            state = ClientState.WAITING;

            try
            {
                //Send a first message so that the other client knows we are here and stops waiting
                SendMessage(MessageType.CONNECT);
            }
            catch
            {
                Debug.Log(string.Concat(username,": Other client is not available yet"));
            }

        }
    }

    public void OnDisconnect()
    {
        SendMessage(MessageType.DISCONNECT);

        Disconnect();
    }

    public void Disconnect() //What to do when disconnecting
    {
        state = ClientState.LOGIN;

        loginMenu.SetActive(true);
        GameMenu.SetActive(false);

        socket.Close();
    }

    void StartGame()
    {
        player = GameObject.Find("Player1");
        //player.GetComponent<Material>().color = color;
        //GameObject.Find("Player1Name").GetComponent<TextMeshPro>().text = username;

        enemyPlayer = GameObject.Find("Player2");
        //enemyPlayer.GetComponent<Material>().color = enemyColor;
        //GameObject.Find("Player2Name").GetComponent<TextMeshPro>().text = enemyUsername;
    }
}

