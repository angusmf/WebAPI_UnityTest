using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
//using IO.Swagger.Api;
using LanGameUsers.Api;
using System;



public class Player : NetworkBehaviour
{

    string tokenVal = string.Empty;
    bool connected = false;


    private void Awake()
    {
        //Put Receives first in awake
        MessageBroker.Default.Receive<SendAuthTokenToServer>().Subscribe(token =>
        {
            Debug.Log("Player got SendAuthTokenToServer");
            Login(token.tokenVal);
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        //Put Publishes in Start so they come after Receive subs exist
        if (!isServer)
        {
            MessageBroker.Default.Publish(new PlayerStarted());
        }
    }

    void Login(string token)
    {
        Debug.Log("called Login on player");
        Cmd_SendAuthToken(token);
    }

    [Command]
    void Cmd_SendAuthToken(string token)
    {
        Debug.Log(string.Format("Received token {0} on server", token));
        MessageBroker.Default.Publish(new ClientAuthTokenReceived(token, this));
    }

    [ClientRpc]
    void Rpc_YouAreLoggedIn()
    {
        Debug.Log("you are successfully logged in on the game server!");
    }
}



