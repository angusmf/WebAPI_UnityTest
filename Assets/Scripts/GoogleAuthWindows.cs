﻿// Copyright 2016 Google Inc.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using UnityEngine;
using IngameDebugConsole;
using UniRx;
//using IO.Swagger.Api;
//using IO.Swagger.Client;
//using IO.Swagger.Model;
using LanGameUsers.Api;
using LanGameUsers.Client;
using LanGameUsers.Model;

public class GoogleAuthWindows : MonoBehaviour
{


    private void Start()
    {
        if (Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor) return;

        DebugLogConsole.AddCommandStatic("login", "Sign in to this app on this device.", "Auth", typeof(GoogleWindowsSignIn));
       
    }

 
    class GoogleWindowsSignIn
    {

        Configuration _config;
        public static void Auth()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

            
            GoogleWindowsSignIn p = new GoogleWindowsSignIn();
            p.doOAuth();
#else
        return;
#endif

        }

        // client configuration
        const string clientID = "556278931449-22n0bgt12upd91k556dk7hhbkb073hbi.apps.googleusercontent.com";
        const string clientSecret = "Z4Bf58AQ62W0oAn2d42CmjBe";

        const string wellKnownConfigEndpoint = "https://accounts.google.com/.well-known/openid-configuration";

        // ref http://stackoverflow.com/a/3978040
        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private async void doOAuth()
        {

            _config = await getConfig();

            // Generates state and PKCE values.
            string state = randomDataBase64url(32);
            string code_verifier = randomDataBase64url(32);
            string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
            const string code_challenge_method = "S256";

            // Creates a redirect URI using an available port on the loopback address.
            string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());
            //output("redirect URI: " + redirectURI);

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectURI);
            //output("Listening..");
            http.Start();

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format("{0}?response_type=code&scope=email%20openid%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                _config.authorization_endpoint,
                System.Uri.EscapeDataString(redirectURI),
                clientID,
                state,
                code_challenge,
                code_challenge_method);

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest);

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Brings the Console to Focus.
            BringConsoleToFront();

            // Sends an HTTP response to the browser.
            var response = context.Response;
            string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>");
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                //output("HTTP server stopped.");
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                output(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
                return;
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                output("Malformed authorization response. " + context.Request.QueryString);
                return;
            }

            // extracts the code
            var code = context.Request.QueryString.Get("code");
            var incoming_state = context.Request.QueryString.Get("state");

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incoming_state != state)
            {
                output(String.Format("Received request with invalid state ({0})", incoming_state));
                return;
            }
            //output("Authorization code: " + code);

            // Starts the code exchange at the Token Endpoint.
            performCodeExchange(code, code_verifier, redirectURI);
        }

        async void performCodeExchange(string code, string code_verifier, string redirectURI)
        {
            //output("Exchanging code for tokens...");

            // builds the  request
            
            string tokenRequestURI = _config.token_endpoint;
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                code,
                System.Uri.EscapeDataString(redirectURI),
                clientID,
                code_verifier,
                clientSecret
                );

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    string responseText = await reader.ReadToEndAsync();
                    //output(responseText);

                    // converts to dictionary
                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    string access_token = tokenEndpointDecoded["access_token"];
                    string tokenId = tokenEndpointDecoded["id_token"];
                    Authorize(tokenId);
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        output("HTTP: " + response.StatusCode);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            string responseText = await reader.ReadToEndAsync();
                            output(responseText);
                        }
                    }

                }
            }
        }


        private async void Authorize(string tokenId)
        {
            output(string.Format("GoogleAuth.Authorize called: {0}", tokenId));

            var apiInstance = new AuthApi("https://auth.angusmf.com");
            //var apiInstance = new AuthApi("http://127.0.0.1:4000");


            try
            {
                var response = await apiInstance.GoogleAsyncWithHttpInfo(new UserView(tokenId));

                if (response.StatusCode == 200)
                {
                    output(string.Format("Success! Token = {0}", response.Data.Token));


                    //MessageBroker.Default.Publish(new ClientAuthTokenReceived(response.Data.Token, null));
                    MessageBroker.Default.Publish(new ServerAuthTokenReceived(response.Data.Token));
                }
                else
                {
                    output(string.Format("Error: Received response code {0} during authorization", response.StatusCode));
                }



            }
            catch (ApiException e)
            {
                output(string.Format("e.Data: {0} \n e.ErrorCode: {1} \n e.ErrorContent: {2} \n e.Message: {3}", e.Data, e.ErrorCode, e.ErrorContent, e.Message));
            }
            catch (Exception e)
            {
                output("Exception when calling apiInstance.AuthenticateWithHttpInfo: " + e.Message);
            }
        }


        class Configuration
        {
            public string issuer;
            public string authorization_endpoint;
            public string token_endpoint;
            public string userinfo_endpoint;
            public string revocation_endpoint;
            public string jwks_uri;
            public List<string> response_types_supported;
            public List<string> subject_types_supported;
            public List<string> id_token_signing_alg_values_supported;
            public List<string> scopes_supported;
            public List<string> token_endpoint_auth_methods_supported;
            public List<string> claims_supported;
            public List<string> code_challenge_methods_supported;

        }

        async Task<Configuration> getConfig()
        {
            // sends the request
            HttpWebRequest configRequest = (HttpWebRequest)WebRequest.Create(wellKnownConfigEndpoint);
            configRequest.Method = "GET";
            configRequest.ContentType = "application/x-www-form-urlencoded";
            configRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";


            try
            {
                // gets the response
                WebResponse configResponse = await configRequest.GetResponseAsync();
                using (StreamReader userinfoResponseReader = new StreamReader(configResponse.GetResponseStream()))
                {
                    // reads response body
                    string configResponseText = await userinfoResponseReader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<Configuration>(configResponseText);
                }
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return null;
            }
        }


        async void userinfoCall(string access_token)
        {
            // sends the request
            HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(_config.userinfo_endpoint);
            userinfoRequest.Method = "GET";
            userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
            userinfoRequest.ContentType = "application/x-www-form-urlencoded";
            userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            // gets the response
            WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
            using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
            {
                // reads response body
                string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
                output(userinfoResponseText);
            }
        }

        /// <summary>
        /// Appends the given string to the on-screen log, and the debug console.
        /// </summary>
        /// <param name="output">string to be appended</param>
        public void output(string output)
        {
            Debug.Log(output);
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        public static string randomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return base64urlencodeNoPadding(bytes);
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        /// <param name="inputStirng"></param>
        /// <returns></returns>
        public static byte[] sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }

        // Hack to bring the Console window to front.
        // ref: http://stackoverflow.com/a/12066376

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public void BringConsoleToFront()
        {
            SetForegroundWindow(GetConsoleWindow());
        }

    }

}