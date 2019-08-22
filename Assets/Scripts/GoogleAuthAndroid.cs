// <copyright file="SigninSampleScript.cs" company="Google Inc.">
// Copyright (C) 2017 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations

namespace SignInSample
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Google;
    using IngameDebugConsole;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UI;
    //using IO.Swagger.Api;
    //using IO.Swagger.Client;
    //using IO.Swagger.Model;
    using LanGameUsers.Api;
    using LanGameUsers.Client;
    using LanGameUsers.Model;

    public class GoogleAuthAndroid : MonoBehaviour
    {

        public string webClientId = "<your client id here>";

        private GoogleSignInConfiguration configuration;

        // Defer the configuration creation until Awake so the web Client ID
        // Can be set via the property inspector in the Editor.
        void Awake()
        {
            configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                UseGameSignIn = false,
                RequestAuthCode = false,
                RequestEmail = true
            };
        }

        private void Start()
        {
            if (Application.platform != RuntimePlatform.Android) return;

            DebugLogConsole.AddCommandInstance("login","Sign in to this app on this device.", "OnSignIn", this);
            DebugLogConsole.AddCommandInstance("logout", "Sign out of app. Will require sign in to connect in the future.", "OnSignOut", this);
            DebugLogConsole.AddCommandInstance("disc", "Disconnect your account from this game. Will require re-authorization to connect in the future.", "OnDisconnect", this);
            DebugLogConsole.AddCommandInstance("silent", "Sign in without a login dialog if possible.", "OnSignInSilently", this);

        }

        public void OnSignIn()
        {
            GoogleSignIn.Configuration = configuration;            
            LogMsg("Calling SignIn");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
              OnAuthenticationFinished);
        }

        public void OnSignOut()
        {
            LogMsg("Calling SignOut");
            GoogleSignIn.DefaultInstance.SignOut();
        }

        public void OnDisconnect()
        {
            LogMsg("Calling Disconnect");
            GoogleSignIn.DefaultInstance.Disconnect();
        }

        internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
        {
            if (task.IsFaulted)
            {
                using (IEnumerator<System.Exception> enumerator =
                        task.Exception.InnerExceptions.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        GoogleSignIn.SignInException error =
                                (GoogleSignIn.SignInException)enumerator.Current;
                        LogMsg("Authentication got Error: " + error.Status + " " + error.Message);
                    }
                    else
                    {
                        LogMsg(task.Exception.ToString());
                    }
                }
            }
            else if (task.IsCanceled)
            {
                LogMsg("Authentication canceled");
            }
            else
            {
                LogMsg("Welcome: " + task.Result.DisplayName + "!");
                Authorize(task.Result.IdToken);
            }
        }

        public void OnSignInSilently()
        {
            GoogleSignIn.Configuration = configuration;
            LogMsg("Calling SignIn Silently");

            GoogleSignIn.DefaultInstance.SignInSilently()
                  .ContinueWith(OnAuthenticationFinished);
        }

        void LogMsg(string msg)
        {
            MessageBroker.Default.Publish(new DebugMsg(msg));
        }


        private async void Authorize(string tokenId)
        {
            LogMsg(string.Format("GoogleAuth.Authorize called: {0}", tokenId));

            var apiInstance = new AuthApi("https://auth.angusmf.com");
       

            try
            {
                var response = await apiInstance.GoogleAsyncWithHttpInfo(new UserView(tokenId));
 
                if (response.StatusCode == 200)
                {
                    LogMsg(string.Format("Success! Token = {0}", response.Data.Token));


                    //MessageBroker.Default.Publish(new ClientAuthTokenReceived(response.Data.Token, null));
                    MessageBroker.Default.Publish(new ServerAuthTokenReceived(response.Data.Token));
                }
                else
                {
                    LogMsg(string.Format("Error: Received response code {0} during authorization", response.StatusCode));
                }



            }
            catch (ApiException e)
            {
                LogMsg(string.Format("e.Data: {0} \n e.ErrorCode: {1} \n e.ErrorContent: {2} \n e.Message: {3}", e.Data, e.ErrorCode, e.ErrorContent, e.Message));
            }
            catch (Exception e)
            {
                LogMsg("Exception when calling apiInstance.AuthenticateWithHttpInfo: " + e.Message);
            }
        }

    }
}
