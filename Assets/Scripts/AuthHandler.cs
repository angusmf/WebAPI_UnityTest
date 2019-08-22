//using IO.Swagger.Api;
using LanGameUsers.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class AuthHandler : MonoBehaviour
{
    void Awake()
    {
        var tokenReceived = MessageBroker.Default.Receive<ClientAuthTokenReceived>();
        tokenReceived.Subscribe(AuthorizeConnection);
        tokenReceived.Subscribe(TestRoles);

        var serverToken = MessageBroker.Default.Receive<ServerAuthTokenReceived>();
        serverToken.Subscribe(AuthorizeServerConn);
        serverToken.Subscribe(ServerTestRoles);
        //serverToken.Subscribe(ServerTestBoth);
    }


    private void AuthorizeServerConn(ServerAuthTokenReceived obj)
    {
        MsgLogger.LogMsg("AuthorizeServerConn!!");
        var apiInstance = new UsersApi("https://auth.angusmf.com");
        apiInstance.Configuration.AddDefaultHeader("Authorization", string.Format("Bearer {0}", obj.tokenVal));


        try
        {
            var response = apiInstance.GetIdWithHttpInfo();
            if (response.StatusCode == 200)
            {
                MsgLogger.LogMsg(string.Format("user id = {0}", response.Data.Id));

                return;
            }
            else
            {
                MsgLogger.LogMsgError(string.Format("Error: Received response code {0} during GetIdWithHttpInfo", response.StatusCode));
            }
        }
        catch (Exception e)
        {
            MsgLogger.LogMsgError("Exception when calling apiInstance.GetIdWithHttpInfo: " + e.Message);
        }
    }

    private void AuthorizeConnection(ClientAuthTokenReceived obj)
    {

        MsgLogger.LogMsg("AuthorizeConnection!!");
        var apiInstance = new UsersApi("https://auth.angusmf.com");
        apiInstance.Configuration.AddDefaultHeader("Authorization", string.Format("Bearer {0}", obj.token));


        try
        {
            var response = apiInstance.GetIdWithHttpInfo();
            if (response.StatusCode == 200)
            {
                MsgLogger.LogMsg(string.Format("user id = {0}", response.Data.Id));

                return;
            }
            else
            {
                obj.player.connectionToClient.Disconnect();
                MsgLogger.LogMsgError(string.Format("Error: Received response code {0} during GetIdWithHttpInfo", response.StatusCode));
            }
        }
        catch (Exception e)
        {
            obj.player.connectionToClient.Disconnect();
            MsgLogger.LogMsgError("Exception when calling apiInstance.GetIdWithHttpInfo: " + e.Message);
        }
    }

    private void TestRoles(ClientAuthTokenReceived obj)
    {

        MsgLogger.LogMsg("TestRoles!!");
        var apiInstance = new UsersApi("https://auth.angusmf.com");
        apiInstance.Configuration.AddDefaultHeader("Authorization", string.Format("Bearer {0}", obj.token));


        try
        {
            var response = apiInstance.GetAllWithHttpInfo();
            if (response.StatusCode == 200)
            {
                MsgLogger.LogMsg(string.Format("user count = {0}", response.Data.Count));

                return;
            }
            else
            {
                MsgLogger.LogMsgError(string.Format("Error: Received response code {0} during GetAllWithHttpInfo", response.StatusCode));
            }
        }
        catch (Exception e)
        {
            MsgLogger.LogMsgError("Exception when calling apiInstance.GetAllWithHttpInfo: " + e.Message);
        }
    }


    private void ServerTestRoles(ServerAuthTokenReceived obj)
    {

        MsgLogger.LogMsg("ServerTestRoles!!");
        var apiInstance = new UsersApi("https://auth.angusmf.com");
        apiInstance.Configuration.AddDefaultHeader("Authorization", string.Format("Bearer {0}", obj.tokenVal));


        try
        {
            var response = apiInstance.GetAllWithHttpInfo();
            if (response.StatusCode == 200)
            {
                MsgLogger.LogMsg(string.Format("user count = {0}", response.Data.Count));

                return;
            }
            else
            {
                MsgLogger.LogMsgError(string.Format("Error: Received response code {0} during GetAllWithHttpInfo", response.StatusCode));
            }
        }
        catch (Exception e)
        {
            MsgLogger.LogMsgError("Exception when calling apiInstance.GetAllWithHttpInfo: " + e.Message);
        }
    }

    private void ServerTestBoth(ServerAuthTokenReceived obj)
    {

        MsgLogger.LogMsg("ServerTestBoth!!");
        var apiInstance = new UsersApi("https://auth.angusmf.com");
        apiInstance.Configuration.AddDefaultHeader("Authorization", string.Format("Bearer {0}", obj.tokenVal));


        try
        {
            var response = apiInstance.GetAllWithHttpInfo();
            if (response.StatusCode == 200)
            {
                MsgLogger.LogMsg(string.Format("(Server)GetAllWithHttpInfo = {0}", response.Data));

                return;
            }
            else
            {
                MsgLogger.LogMsgError(string.Format("Error: Received response code {0} during GetAllWithHttpInfo", response.StatusCode));
            }
        }
        catch (Exception e)
        {
            MsgLogger.LogMsgError("Exception when calling apiInstance.GetAllWithHttpInfo: " + e.Message);
        }


        try
        {
            var response = apiInstance.GetIdWithHttpInfo();
            if (response.StatusCode == 200)
            {
                MsgLogger.LogMsg(string.Format("(Server)GetIdWithHttpInfo = {0}", response.Data));

                return;
            }
            else
            {
                MsgLogger.LogMsgError(string.Format("Error: Received response code {0} during GetIdWithHttpInfo", response.StatusCode));
            }
        }
        catch (Exception e)
        {
            MsgLogger.LogMsgError("Exception when calling apiInstance.GetIdWithHttpInfo: " + e.Message);
        }
    }


}
