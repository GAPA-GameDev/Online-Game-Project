using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatMenu : MonoBehaviour
{

    public GameObject serverStatusGO;
    public GameObject UI;


   public void logIn()
    {


        if (serverStatusGO.GetComponent<ClientScript>().getSocketStatus())
        {
            UI.SetActive(true);
            gameObject.SetActive(false);


        }




    }
}
