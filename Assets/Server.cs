using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;

using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.IO;
using System.Xml.Serialization;

using UnityEngine.Networking;

public class Server : MonoBehaviour
{

    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    public int port = 9050; // Port 
    public int otherClientPort = 9090; //For now we are using another local client (A replica of this)

    UdpClient socket; //UdpClient for this specific client, its port is set as the public port variable

    IPEndPoint clientIP; //Initialize this UdpClient with this IPEndPoint
    IPEndPoint otherClientIP;

    string host = "127.0.0.1";

    private bool serverStarted;

    string SMstring = "$SM|";

    private Commands commands;

    // Start is called before the first frame update
    void Start()
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {

            clientIP = new IPEndPoint(IPAddress.Any,port);
            socket = new UdpClient(port); //This binds the IPEndPoint

            serverStarted = true; //set bool 

            commands = new Commands();

            // For now we add the local client we have in another scene
            otherClientIP = new IPEndPoint(IPAddress.Parse(host),otherClientPort); 
            ServerClient localClient = new ServerClient(new UdpClient(otherClientIP));
            localClient.ip = otherClientIP;
            clients.Add(localClient);

        }
        catch (Exception e)
        {

            Console.WriteLine("Error startign server:" + e.Message); //Print if error is catched

        }
    }

    // Update is called once per frame
    void Update() //this update funtion should be going in a thread
    {
        if (!serverStarted)
            return;

        for (int i = 0; i < clients.Count; i++)//lets check for incomming messages for each serverclient 
        {


            if (!Isconnected(clients[i].udpClient))// check if the client is connected if not:
            {
                clients[i].udpClient.Close();
                disconnectList.Add(clients[i]);
                continue;

            }
            else// if connected then 
            {

                //Check with a poll if clients have sent anything and process the info (Maybe we just need to check for this specific's client message)
                



            }

        }

        for (int i = 0; i < disconnectList.Count - 1; i++)
        {

            Broadcast(SMstring + disconnectList[i].clientName + "has disconnected", clients);

            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);

        }
    }

    private void OnIncomingData(ServerClient c, ClientMessage message)
    {
        //  Console.WriteLine(c.clientName + "has sent the following message:" + data);

        if (message.messageContent != null)
        {

            if (message.messageContent.Contains("&NAME"))
            {

                commands.RegisterNameCM(SMstring, message.messageContent, c, clients);

            }
            else if (message.messageContent.Contains("&HELP"))
            {
                commands.HelpCM(c);
            }
            else if (message.messageContent.Contains("&LIST"))
            {

                commands.ListCM(c, clients);

            }
            else if (message.messageContent.Contains("&WHISPER"))//EXAMPLE---> $WHISPER|Adrian:hello there? 
            {

                commands.WhisperCM(message.messageContent, c, clients);


            }
            else
            {
                Broadcast(c.clientName + ":" + message.messageContent, clients);
            }
        }

    }

    private bool Isconnected(UdpClient udpClient)  // tip: TcpClient.Client equal to the socket
    {

        try// maybe we are not able to reach a client 
        {

            if (udpClient != null && udpClient.Client != null && udpClient.Client.Connected)//tcp is null means the TcpClient does not have any data assigned // if tcp.client is different from null then we have a socket connected// and tcp.Client.Connected is true when the client is connected thorugh a remote resource since the last operation 
            {

                if (udpClient.Client.Poll(0, SelectMode.SelectRead))//The Poll method checks the state of the Socket. IMPORTANT Poll first paramenter is the time to wait for a respons in MICROSECONDS https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.poll?view=net-5.0
                {
                    return !(udpClient.Client.Receive(new byte[1], SocketFlags.Peek) == 0);   // if the signal sent is not zero (meaning the client wants to disconnect)then send true  

                }
                return true;
            }
            else
            {
                Console.WriteLine("Error when reaching client Socket is null or not connected");
                return false;

            }

        }
        catch
        {
            Console.WriteLine("Client has not been reach");
            return false;
        }

    }

    private void AcceptTcpClient(IAsyncResult ar) //AsyncCallback to store clients in lists, 
    {

        TcpListener listener = (TcpListener)ar.AsyncState; //This property returns the object that is the last parameter of the method that initiates an asynchronous operation.

        //clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));//EndAcceptTcpClient End asynchronous operation 
        //StartListening();//once a client has been accepted then we wait for another conncetion 


        // Here we should display a connection in the chat, e.j. Adrián has connected

        //Broadcast(clients[clients.Count-1].clientName + "has connected",clients);
        Broadcast("%NAME", new List<ServerClient>() { clients[clients.Count - 1] }); // IMPROVE We should create overload for broadcasting

    }

    internal void Broadcast(string data, List<ServerClient> clientL)
    {

        foreach (ServerClient c in clientL)
        {

            try
            {
                //get client stream
                //NetworkStream stream = c.udpClient.GetStream();
                //data passed as an argument to Broadcast()
                //encode data to send to the user
                var message = new ClientMessage();
                message.messageContent = data;
                message.clientName = c.clientName;

                byte[] tmp = SerializeMessage(message);

                //TODO: Send message to all clients
                socket.Send(tmp,tmp.Length,c.ip);


            }
            catch (Exception e)
            {
                Console.WriteLine("Write error :" + e.Message + "To client " + c.clientName);

            }
        }


    }
    internal void Broadcast(string data, ServerClient c)
    {



        try
        {
            


        }
        catch (Exception e)
        {
            Console.WriteLine("Write error :" + e.Message + "To client " + c.clientName);

        }



    }

    public void OnServerShutDown()
    {

        //We disconnect each client
        foreach (ServerClient sc in clients)
        {
            //sc.tcp.Close();
            //disconnectList.Add(sc);

        }
        clients.Clear();

        Console.WriteLine("clients disconnocted from server");

        //server.Stop();
        serverStarted = false;
        Console.WriteLine("Server conncetion closed");




    }

    public bool GetServerStatus() { return serverStarted; }


    byte[] SerializeMessage(ClientMessage message)
    {
        
        string tmp = JsonUtility.ToJson(message);

        return Encoding.ASCII.GetBytes(tmp);
    }

    public ClientMessage DeserializeMessage(Stream stream)
    {
        TextReader textReader = new StreamReader(stream);
        string longString = string.Empty;
        while (true)
        {
            string s = string.Empty;
            try
            {
                s = textReader.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading string:" + e.Message);
                break;
            }
            if (s == null)
                break;

            longString = longString + s;

            if (s.Contains("clientName")) // name is the last parameter so it is the end of the class (last line to read)
                break;

        }
        longString = longString + "</ClientMessage>"; //I don't know why the last line pops an error so i'll add it myself

        XmlSerializer serializer = new XmlSerializer(typeof(ClientMessage));
        return (ClientMessage)serializer.Deserialize(new StringReader(longString));
    }
}




class Commands : Server
{

    public void RegisterNameCM(string SM, string data, ServerClient c, List<ServerClient> list)
    {
        c.clientName = data.Split('|')[1];
        Broadcast(SM + c.clientName + " has connected", list);
        Console.WriteLine("Client Identify as :" + c.clientName);



    }

    public void HelpCM(ServerClient c)
    {

        //Broadcast("&LIST To see players connected," +
            //"&WHISPER to talk directly to a player,", c);//TODO: write correct message


    }
    public void ListCM(ServerClient c, List<ServerClient> list)
    {
        string aux = null;

        foreach (ServerClient sc in list)
        {

            if (aux == null)
                aux = sc.clientName + "@";
            else
                aux = aux + "@" + sc.clientName + "@";

        }

        aux = aux.Replace("@", System.Environment.NewLine);
        Broadcast(aux, c);
        return;



    }
    public void WhisperCM(string data, ServerClient c, List<ServerClient> list)
    {

        string[] Auxdata = data.Split('|', ':');

        string user = Auxdata[1];
        string message = Auxdata[2];

        foreach (ServerClient sc in list)
        {

            if (sc.clientName == user)
            {

                Broadcast(c.clientName + ":" + message, sc);
                break;
            }
        }


    }
    private void OnDestroy()
    {
        OnServerShutDown();
    }


}

public class ServerClient // we need this class to store a list of clients, who are those which are connected 
{
    public UdpClient udpClient; //socket assignation 
    public IPEndPoint ip; //IPEndPoint of each client
    public string clientName;// client name

    public ServerClient(UdpClient clientSocket)//contructor where we define the client name and the tcp socket 
    {
        clientName = "Guest";
        udpClient = clientSocket;
    }



}

public struct ClientMessage // struct to serialize when sending a message. 
{
    public string messageContent;
    public string clientName;
    //color
}