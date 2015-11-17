/**
 * Copyright 2015 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Activiti.Client
{
    public class RestClient
    {
        #region Constants and Properties
        // DFTODO - define properties such as Username, Password, Server, ...
        //private const String URIBASE = "activiti-rest";
        private const int TIMEOUTSEC = 90;
        private const String CONTENTTYPE = "application/json";
        private const String AUTHORIZATIONHEADERFORMAT = "Basic {0}";
        private const String USERAGENT = "d-fens biz.dfch.CS.Activiti.Client.RestClient";

        public string Username { get; set; }

        private string _password;
        public string Password 
        { 
            set
            {
                _password = value;
            }
            private get
            {
                return _password;
            }
        }

        private NetworkCredential _Credential;
        private String _CredentialBase64;
        public NetworkCredential Credential
        {
            get
            {
                return _Credential;
            }
            set
            {
                _Credential = value ?? (new NetworkCredential(String.Empty, String.Empty));
                var abCredential = System.Text.UTF8Encoding.UTF8.GetBytes(String.Format("{0}:{1}", _Credential.UserName, _Credential.Password));
                _CredentialBase64 = Convert.ToBase64String(abCredential);
            }
        }
        public void SetCredential(String username, String password)
        {
            this.Credential = new NetworkCredential(username, password);
        }

        private String _ContentType = CONTENTTYPE;
        public String ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        private Uri _UriServer;
        public Uri UriServer
        {
            get { return _UriServer; }
            set { _UriServer = value; }
        }

        private int _TimeoutSec = TIMEOUTSEC;
        public int TimeoutSec
        {
            get
            {
                return _TimeoutSec;
            }
            set
            {
                _TimeoutSec = value;
            }
        }

        #endregion

        #region Constructor And Initialisation
        public RestClient()
        {
            // N/A
        }

        public RestClient(Uri server, string username, string password)
        {
            Contract.Requires(null != server);
            Contract.Requires(!string.IsNullOrWhiteSpace(username));
            Contract.Requires(!string.IsNullOrWhiteSpace(password));

            UriServer = server;
            Username = username;
            Password = password;
            this.Initialise(UriServer, Username, Password);
        }

        private void Initialise(Uri server, string username, string password)
        {
            this.UriServer = server;
            this.Username = username;
            this.Password = password;
            SetCredential(Username, Password);
        }
        #endregion

        #region Functional Methods
        public void Login()
        {
            string uri = String.Format("identity/users/{0}", HttpUtility.UrlEncode(Username));
            string login = Invoke(uri);
        }
        }

        #endregion

        #region Invoke
        public String Invoke(
            String method
            ,
            String uri
            ,
            Hashtable queryParameters
            ,
            Hashtable headers
            ,
            String body
            )
        {
            // Parameter validation
            if (String.IsNullOrWhiteSpace(method)) throw new ArgumentException(String.Format("Method: Parameter validation FAILED. Parameter cannot be null or empty."), "Method");
            if (String.IsNullOrWhiteSpace(uri)) throw new ArgumentException(String.Format("Uri: Parameter validation FAILED. Parameter cannot be null or empty."), "Uri");

            headers = headers ?? (new Hashtable());
            queryParameters = queryParameters ?? (new Hashtable());

            Debug.WriteLine(String.Format("Invoke: UriServer '{0}'. TimeoutSec '{1}'. Method '{2}'. Uri '{3}'.", _UriServer.AbsoluteUri, _TimeoutSec, method, uri));
            if (null == Credential)
            {
                Debug.WriteLine(String.Format("No Credential specified."));
            }
            else
            {
                if (String.IsNullOrWhiteSpace(Credential.Password))
                {
                    Debug.WriteLine(String.Format("Username '{0}', Password '{1}'.", Credential.UserName, "*"));
                }
                else
                {
                    Debug.WriteLine(String.Format("Username '{0}', Password '{1}'.", Credential.UserName, "********"));
                }
            }

            using (var cl = new HttpClient())
            {
                char[] achTrim = { '/' };
                //var s = String.Format("{0}/{1}/", _UriServer.AbsoluteUri.TrimEnd(achTrim), _UriBase.Trim(achTrim));
                var s = String.Format("{0}/", _UriServer.AbsoluteUri.TrimEnd(achTrim));
                cl.BaseAddress = new Uri(s);
                cl.Timeout = new TimeSpan(0, 0, _TimeoutSec);
                cl.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_ContentType));
                cl.DefaultRequestHeaders.Add("Authorization", String.Format(RestClient.AUTHORIZATIONHEADERFORMAT, _CredentialBase64));
                cl.DefaultRequestHeaders.Add("User-Agent", RestClient.USERAGENT);

                var queryParametersString = "?";
                foreach (DictionaryEntry item in queryParameters)
                {
                    queryParametersString += String.Format("{0}={1}&", item.Key, item.Value);
                }
                char[] achTrimAmp = { '&' };
                queryParametersString = queryParametersString.TrimEnd(achTrimAmp);

                // Invoke request
                HttpResponseMessage response;
                var _method = new HttpMethod(method);
                uri += queryParametersString;
                char[] achTrimQuestion = { '?' };
                uri = uri.TrimEnd(achTrimQuestion);
                switch (_method.ToString().ToUpper())
                {
                    case "GET":
                    case "HEAD":
                        response = cl.GetAsync(uri).Result;
                        break;
                    case "POST":
                        {
                            var _body = new StringContent(body);
                            _body.Headers.ContentType = new MediaTypeHeaderValue(_ContentType);
                            response = cl.PostAsync(uri, _body).Result;
                        }
                        break;
                    case "PUT":
                        {
                            var _body = new StringContent(body);
                            _body.Headers.ContentType = new MediaTypeHeaderValue(_ContentType);
                            response = cl.PutAsync(uri, _body).Result;
                        }
                        break;
                    case "DELETE":
                        response = cl.DeleteAsync(uri).Result;
                        break;
                    default:
                        throw new NotImplementedException(String.Format("{0}: Method not implemented. Currently only the following methods are implemented: 'GET', 'HEAD', 'POST', 'PUT', 'DELETE'.", _method.ToString().ToUpper()));
                }
                if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                {
                    throw new UnauthorizedAccessException(String.Format("Invoking '{0}' with username '{1}' FAILED.", uri, _Credential.UserName));
                }
                if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    var message = String.Empty;
                    var contentError = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        /*
                        JToken jv = JObject.Parse(contentError);
                        var messageError = jv.SelectToken("Error", true).ToString();
                        var messageCode = jv.SelectToken("code", true).ToString();
                        var messageText = jv.SelectToken("text", true).ToString();
                        message = String.Format("{0}\r\nCode: {1}\r\nText: {2}", messageError, messageCode, messageText);
                         * */
                        message = contentError;
                    }
                    catch
                    {
                        message = contentError;
                    }
                    throw new ArgumentException(message);
                }
                response.EnsureSuccessStatusCode();

                Debug.WriteLine(String.Format("response '{0}'", response.ToString()));
                var content = response.Content.ReadAsStringAsync().Result;
                Debug.WriteLine(String.Format("content '{0}'", content.ToString()));

                return content;
            }
        }

        public String Invoke(
            String uri
            ,
            Hashtable queryParameters
            )
        {
            return this.Invoke(HttpMethod.Get.ToString(), uri, queryParameters, null, null);
        }

        public String Invoke(
            String uri
            )
        {
            return this.Invoke(HttpMethod.Get.ToString(), uri, null, null, null);
        }
        #endregion

    }
}
