using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using IO.Swagger.Api;
//using IO.Swagger.Client;
//using IO.Swagger.Model;
using LanGameUsers.Api;
using LanGameUsers.Client;
using LanGameUsers.Model;
using System;
using TMPro;
using UniRx;



public class APITest : MonoBehaviour

{

    private void Start()
    {
        MessageBroker.Default.Receive<StartClient>().Subscribe(_ => Authorize());
        MessageBroker.Default.Receive<StartServer>().Subscribe(_ => Authorize());
    }

private void Authorize()
{

        var apiInstance = new UsersApi("http://192.168.0.116:4000");
        //var userDto = new UserDto();

        //userDto.Username = "angusmf@gmail.com";
        ////userDto.Username = "ang";
        //userDto.Password = "P@ssword123";


        //try
        //{
        //    apiInstance.Register(userDto);
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError("Exception when calling apiInstance.Register: " + e.Message);
        //}

        //try
        //{
        //    var response = apiInstance.AuthenticateWithHttpInfo<AuthContent>(userDto);
        //    if (response.StatusCode == 200)
        //    {
        //        Debug.Log(string.Format("Token = {0}", response.Data.Token));


        //        MessageBroker.Default.Publish(new AuthTokenReceived(response.Data.Token));
        //        MessageBroker.Default.Publish(new ServerAuthTokenReceived(response.Data.Token));
        //    }
        //    else
        //    {
        //        Debug.LogError(string.Format("Error: Received response code {0} during authorization", response.StatusCode));
        //    }
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError("Exception when calling apiInstance.AuthenticateWithHttpInfo: " + e.Message);

        //}
    }

}
