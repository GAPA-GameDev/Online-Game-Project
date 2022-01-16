using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviourPunCallbacks
{

    public GameObject mainMenu;
    public GameObject loadingMenu;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(true);
        loadingMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlay()
    {
        PhotonNetwork.ConnectUsingSettings();
        mainMenu.SetActive(false);
        loadingMenu.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
