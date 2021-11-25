using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using System.Xml.Serialization;

public class ClientScript : MonoBehaviour
{

    public GameObject chatContainer;
    public GameObject messagePrefab;
    public GameObject ServerContainer;
    public GameObject SMPrefab;
    public string clientName; 

    private bool socketReady;
    private UdpClient socket;
    private NetworkStream stream;
    

    public void ConnectToServer()
    {

        //if already connected:
        if (socketReady)
            return;

        //Default host / prot values
        string host = "127.0.0.1";
        int port = 9050;
        clientName = "Guest";

        //Overwrite default host/port values
        OverwriteDefValues(host, port);


        try
        {

            socket = new UdpClient(host,port);
            //stream = socket.GetStream();

            socketReady = true; 

        }
        catch(Exception e)
        {

            Debug.Log("SocketError:"+e.Message);

        }
    }


    private void Update()
    {

        if (socketReady)
        {


            if (stream.DataAvailable)
            {

                var message = new ClientMessage(); //Created Message struct in order to pass serialized data
                message = DeserializeMessage(stream);//Deserialize data, in order to use it 

                OnIncomingData(message);


            }


        }



    }

    private void OnIncomingData(ClientMessage message)
    {
        //Debug.Log("Server :"+data);
        if (message.messageContent.ToString() != null)
        {
            if (message.messageContent.ToString().Contains("%NAME"))
            {

                Send("&NAME|" + clientName);
               

            }
            else if (message.messageContent.ToString().Contains("$SM|"))
            {

                message.messageContent = message.messageContent.ToString().Split('|')[1];
                GameObject goSM = Instantiate(SMPrefab, ServerContainer.transform);
                goSM.GetComponentInChildren<TextMeshProUGUI>().text = message.messageContent;
               


            }
            else
            {

                GameObject go = Instantiate(messagePrefab, chatContainer.transform);
                go.GetComponentInChildren<TextMeshProUGUI>().text = message.messageContent.ToString();

            }
        }
       

    }


    private void Send(string data)
    {


        if (!socketReady)
            return;
        else
        {
            var message = new ClientMessage();//new struct unfilled with data
            message.messageContent = data; // defining properties:
            message.clientName = clientName;
            SerializeMessage(stream, message); // Serialize message


        }


    }

    public void OnSendButton()
    {
        string message = GameObject.Find("InputBox").GetComponent<InputField>().text;
        Send(message);


    }

    private void OverwriteDefValues(string s,int i)
    {

        string h;
        int p;
        string n;
        h = GameObject.Find("HostInput").GetComponent<TMP_InputField>().text;
        if (h != "")
            s = h;
        int.TryParse(GameObject.Find("PortInput").GetComponent<TMP_InputField>().text, out p);
        if (p != 0)
            i = p;

        n = GameObject.Find("UserName").GetComponent<TMP_InputField>().text;
        if (n != "")
            clientName = n;



    }

    private void CloseSocket()
    {
        if (!socketReady)
            return;

        socket.Close();
        socketReady = false;

    }

    private void OnApplicationQuit()
    {

        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }

    public struct ClientMessage // struct to serialize when sending a message. 
    {
        public string messageContent;
        public string clientName;
        //color
    }

    public void SerializeMessage(Stream stream, ClientMessage message)
    {
        XmlSerializer clientMessageSerializer = new XmlSerializer(typeof(ClientMessage));

        clientMessageSerializer.Serialize(stream, message); //From what i understand, this method serializes the data and uses the stream to send it

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
                Debug.Log("Error reading string:" + e.Message);
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


    public bool getSocketStatus()
    {
        return socketReady;


    }

}
