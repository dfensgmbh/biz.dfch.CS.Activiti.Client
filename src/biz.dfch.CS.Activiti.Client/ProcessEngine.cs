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
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net;

namespace biz.dfch.CS.Activiti.Client
{
    public class ProcessEngine
    {
        #region Constants and Properties
        private RestClient _Client;
        public RestClient Client
        {
            get
            {
                return _Client;
            }
            set
            {
                _Client = value;
            }
        }
        public string ApplicationName { get; set; }

        #endregion

        #region Constructor And Initialisation
        public ProcessEngine()
        {
            // N/A
        }

        public ProcessEngine(RestClient client)
        {
            Client = client;
        }

        public ProcessEngine(Uri server, string username, string password)
        {
            Client = new RestClient(server, username, password);
        }

        #endregion

        #region Functional Methods
        public void Login()
        {
            var uri = string.Format("identity/users/{0}", HttpUtility.UrlEncode(Client.Credential.UserName));
            var response = Client.Invoke(uri);
        }

        public void Login(string username, string password)
        {
            Client.Username = username;
            Client.Password = password;
            Client.SetCredential(username, password);
            Login();
        }

        public void Login(NetworkCredential credential)
        {
            Client.Credential = credential;
            Login();
        }

        public void Logout()
        {
            Client.Credential = new NetworkCredential(String.Empty, String.Empty);
            Client.Username = null;
            Client.Password = null;
        }

        public T GetWorkflowDefinitions<T>()
        {
            var uri = string.Format("repository/process-definitions");
            var response = Client.Invoke(uri);

            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public object GetWorkflowDefinitions(Type type)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "GetWorkflowDefinitions" && m.IsGenericMethod)).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] {/*parameters*/});
            return result;
        }
        
        public object GetWorkflowDefinitions(object type)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "GetWorkflowDefinitions" && m.IsGenericMethod)).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] {/*parameters*/});
            return result;
        }

        public object GetWorkflowDefinitions()
        {
            var result = GetWorkflowDefinitions<ProcessDefinitionsResponse>();
            return result;
        }

        public T InvokeWorkflowInstance<T>(string definitionId, List<ProcessVariableData> variables)
        {
            var uri = string.Format("runtime/process-instances");
            var request = new ProcessInstanceRequestData()
            {
                processDefinitionId = definitionId,
                businessKey = ApplicationName
            };
            request.variables = variables;
            var jrequest = JsonConvert.SerializeObject(request);

            Debug.WriteLine(string.Format("{0} Body {1}", "POST", jrequest));
            var response = Client.Invoke("POST", uri, null, null, jrequest);
            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public object InvokeWorkflowInstance(string definitionId, Hashtable variablesHt)
        {
            var variables = new List<ProcessVariableData>();
            foreach (DictionaryEntry variable in variablesHt)
            {
                variables.Add(new ProcessVariableData()
                {
                    name = variable.Key.ToString(),
                    value = variable.Value.ToString()
                }
                );
            }
            var result = InvokeWorkflowInstance<ProcessInstanceResponseData>(definitionId, variables);
            return result;
        }

        public T GetWorkflowInstances<T>()
        {
            var uri = string.Format("runtime/process-instances");
            var response = Client.Invoke(uri);

            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public GetWorkflowInstances()
        {
            var result = GetWorkflowInstances<ProcessInstancesResponse>();
            return result;
        }

        public object GetWorkflowInstance(string id)
        {
            var uri = string.Format("runtime/process-instances/{0}", id);
            var response = Client.Invoke(uri);

            var result = JsonConvert.DeserializeObject<ProcessInstancesResponse>(response);
            return result;
        }

        public object UpdateWorkflowInstance(string id)
        {
            var uri = string.Format("runtime/process-instances/{0}", id);
            var response = Client.Invoke(uri);

            var result = JsonConvert.DeserializeObject<ProcessInstancesResponse>(response);
            return result;
        }

        #endregion

    }
}
