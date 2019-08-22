using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;


public class MyNetMan : NetworkManager
{

    private void Start()
    {
        MessageBroker.Default.Receive<StartServer>().Subscribe(_ => StartServer());
        MessageBroker.Default.Receive<StartClient>().Subscribe(_ => StartClient());
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        Debug.Log("Server started");
    }

    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        Debug.Log("Client started");
    }


    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Debug.Log("Client Disconnected");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("Client Connected");

    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
        Debug.Log("Client Error: " + errorCode);
    }

    public override void OnClientNotReady(NetworkConnection conn)
    {
        base.OnClientNotReady(conn);
        Debug.Log("Client Not Ready");
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        Debug.Log("Client Scene Changed");
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        Debug.Log("Server Add Player");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("Server Connect");
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("Server Disconnect");
    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        base.OnServerError(conn, errorCode);
        Debug.Log("Server Error");
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        Debug.Log("Server Ready");
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        Debug.Log("Server Changed Scene: " + sceneName);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
        Debug.Log("Server Remove Player");
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("Client Stopped");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("Server Stopped");
    }
}
