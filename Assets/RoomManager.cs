using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public InputField createRoomInput;
    public InputField joinRoomInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCreateRoom()
    {
        if(createRoomInput.text.Length > 0)
        {
            PhotonNetwork.CreateRoom(createRoomInput.text);
        }
    }

    public void OnJoinRoom()
    {
        if (joinRoomInput.text.Length > 0)
        {
            PhotonNetwork.JoinRoom(joinRoomInput.text);
        }
    }

    public void OnJoinRandomRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        //Load Game Scene 
        SceneManager.LoadScene("GameScene");
    }
}
