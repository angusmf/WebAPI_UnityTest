using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Async;
using UnityEngine.Networking;

public class LoggerInner : MonoBehaviour
{

    string tokenVal = string.Empty;
    bool connected = false;

    Player player;
    // Start is called before the first frame update
    void Start()
    {

        MessageBroker.Default.Receive<AuthTokenReceived>().Subscribe(token =>
        {
            Debug.Log("Token received from web!");
            tokenVal = token.tokenVal;
            if (connected) LogIn(tokenVal);
        });

        MessageBroker.Default.Receive<PlayerStarted>().Subscribe(_ =>
        {
            Debug.Log("Connected!");
            connected = true;
            if (!string.IsNullOrEmpty(tokenVal)) LogIn(tokenVal);
        });

        MessageBroker.Default.Receive<SendAuthTokenToServer>().Subscribe(token =>
        {
            Debug.Log("LoggerInner got SendAuthTokenToServer");
        });

    
        }

  async void LogIn(string token)
    {

        //await UniTask.WaitUntil(() => NetworkManager.singleton.isNetworkActive);
        //await UniTask.DelayFrame(2);

        Debug.Log("called login");
        MessageBroker.Default.Publish(new SendAuthTokenToServer(token));
        //MessageBroker.Default.Publish(new SendAuthTokenToServer("poop"));

        //if (player == null) Debug.LogError("Player is null!");
        //else player.Login(token); 

    }
}
