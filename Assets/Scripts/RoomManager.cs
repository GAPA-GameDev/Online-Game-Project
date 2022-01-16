using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public InputField createRoomInput;
    public InputField joinRoomInput;
    public InputField usernameInput;
    RoomOptions roomOptions;
    // Start is called before the first frame update
    void Start()
    {
        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCreateRoom()
    {
        if(createRoomInput.text.Length > 0)
        {
            if (usernameInput.text.Length > 0)
            {
                PhotonNetwork.LocalPlayer.NickName = usernameInput.text;
            }
           
            PhotonNetwork.CreateRoom(createRoomInput.text, roomOptions);
        }
    }

    public void OnJoinRoom()
    {
        if (joinRoomInput.text.Length > 0)
        {
            if (usernameInput.text.Length > 0)
            {
                PhotonNetwork.LocalPlayer.NickName = usernameInput.text;
            }

            PhotonNetwork.JoinRoom(joinRoomInput.text);
        }
    }

    public void OnJoinRandomRoom()
    {
        if (usernameInput.text.Length > 0)
        {
            PhotonNetwork.LocalPlayer.NickName = usernameInput.text;
        }

        PhotonNetwork.JoinRandomOrCreateRoom(null,2,MatchmakingMode.FillRoom,null,null,null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        //Load Game Scene 
        SceneManager.LoadScene("GameScene");
    }
}
