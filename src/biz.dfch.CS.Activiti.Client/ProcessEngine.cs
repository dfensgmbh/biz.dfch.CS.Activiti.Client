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
    public class ProcessEngine : ContractClassForIBPMService
    {
        #region Constants, Enums and Properties

        public enum EnumStatus
        {
            Suspend,
            Activate
        }

        public enum EnumIndepth
        {
            Executions,
            Tasks
        }

        public string ApplicationName
        {
            get
            {
                return _ApplicationName;
            }
        }

        private Hashtable _QueryParameters(int size = int.MaxValue)
        {
            var uquery = new Hashtable();
            uquery.Add("size", size);
            return uquery;
        }

        #endregion

        #region Constructor And Initialisation

        public ProcessEngine()
            : base()
        {
            // N/A
        }

        public ProcessEngine(Uri server, string applicationName = "", int timeoutSec = 0)
            : base(server, applicationName, timeoutSec)
        {
            Contract.Requires(server != null);
        }

        #endregion

        #region Methods

        #region Login and Logout

        public void Login(string username, string password)
        {
            base.Login(username, password);
            Login();
        }

        public void Login(NetworkCredential credential)
        {
            base.Login(credential);
            Login();
        }

        /// <summary>
        /// Checks if the user exists. If yes, _IsLoggedIn is set to true and user can make requests.
        /// If user does not exist, all further requests will fail.
        /// </summary>
        private void Login()
        {
            if (_IsLoggedIn) return;
            var uri = string.Format("identity/users/{0}", HttpUtility.UrlEncode(_Client.Credential.UserName));
            var response = _Client.Invoke(uri);
            _IsLoggedIn = true;
        }

        /// <summary>
        /// After logout a user cannot do some requests. He has to login again.
        /// </summary>
        public void Logout()
        {
            _Client.Credential = new NetworkCredential(String.Empty, String.Empty);

            _IsLoggedIn = false;
        }

        #endregion Login and Logout end

        #region GetWorkflowDefinition(s)

        public T GetWorkflowDefinitions<T>()
        {
            var uri = string.Format("repository/process-definitions");
            var response = _Client.Invoke(uri, _QueryParameters());

            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public object GetWorkflowDefinitions(Type type)
        {
            Contract.Requires(type != null);

            var mi = this.GetType().GetMethods().Where(m => (m.Name == "GetWorkflowDefinitions" && m.IsGenericMethod)).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] {/*parameters*/});
            return result;
        }

        public object GetWorkflowDefinitions(object type)
        {
            Contract.Requires(type != null);

            var mi = this.GetType().GetMethods().Where(m => (m.Name == "GetWorkflowDefinitions" && m.IsGenericMethod)).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] {/*parameters*/});
            return result;
        }

        public ProcessDefinitionsResponse GetWorkflowDefinitions()
        {
            var result = GetWorkflowDefinitions<ProcessDefinitionsResponse>();
            return result;
        }

        public T GetWorkflowDefinition<T>(string definitionId)
        {
            Contract.Requires(definitionId != null);

            var uri = string.Format("repository/process-definitions/{0}", definitionId);
            var response = _Client.Invoke(uri);

            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public ProcessDefinitionResponseData GetWorkflowDefinition(string definitionId)
        {
            var result = GetWorkflowDefinition<ProcessDefinitionResponseData>(definitionId);
            return result;
        }

        #endregion GetWorkflowDefinition(s) end

        #region InvokeWorkflowInstance

        public T InvokeWorkflowInstance<T>(string definitionId, List<ProcessVariableData> variables)
        {
            Contract.Requires(definitionId != null);

            var uri = string.Format("runtime/process-instances");
            var request = new ProcessInstanceRequestData()
            {
                processDefinitionId = definitionId,
                businessKey = _ApplicationName
            };
            request.variables = variables;
            var jrequest = JsonConvert.SerializeObject(request);

            Debug.WriteLine(string.Format("Body: {0}", jrequest));
            var response = _Client.Invoke("POST", uri, null, null, jrequest);
            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public ProcessInstanceResponseData InvokeWorkflowInstance(string definitionId, Hashtable variablesHt)
        {
            Contract.Requires(definitionId != null);

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

        #endregion InvokeWorkflowInstance end

        #region GetWorkflowInstance(s)

        public T GetWorkflowInstances<T>()
        {
            var uri = string.Format("runtime/process-instances");
            var response = _Client.Invoke(uri, _QueryParameters());

            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public ProcessInstancesResponse GetWorkflowInstances()
        {
            var result = GetWorkflowInstances<ProcessInstancesResponse>();
            return result;
        }

        public T GetWorkflowInstanceVariables<T>(string id)
        {
            Contract.Requires(id != null);

            var uri = string.Format("runtime/process-instances/{0}/variables", id);
            var response = _Client.Invoke(uri, _QueryParameters());

            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public T GetWorkflowInstance<T>(string id)
        {
            Contract.Requires(id != null);

            var uri = string.Format("runtime/process-instances/{0}", id);
            var response = _Client.Invoke(uri);

            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public ProcessInstanceResponseData GetWorkflowInstance(string id)
        {
            var result = GetWorkflowInstance<ProcessInstanceResponseData>(id);
            return result;
        }

        public ProcessInstanceResponseIndepthData GetWorkflowInstance(string id, bool indepth)
        {
            var result = GetWorkflowInstance<ProcessInstanceResponseIndepthData>(id);
            if (indepth == true)
            {
                result.variables = GetWorkflowInstanceVariables<List<ProcessVariableData>>(id);
                var executions = GetWorkflowIndepth<ProcessExecutionsResponse>(id, biz.dfch.CS.Activiti.Client.ProcessEngine.EnumIndepth.Executions);
                result.executions = executions.data;
                foreach (var entry in result.executions)
                {
                    entry.jactivities = GetWorkflowInstanceDetails(String.Format("runtime/executions/{0}/activities", entry.id));
                    //entry.jvariables = GetWorkflowInstanceDetails(String.Format("runtime/executions/{0}/variables", entry.id));
                }
                var tasks = GetWorkflowIndepth<ProcessTasksResponse>(id, biz.dfch.CS.Activiti.Client.ProcessEngine.EnumIndepth.Tasks);
                result.tasks = tasks.data;
                foreach (var entry in result.tasks)
                {
                    entry.jidentitylinks = GetWorkflowInstanceDetails(String.Format("runtime/tasks/{0}/identitylinks", entry.id));
                    entry.jcomments = GetWorkflowInstanceDetails(String.Format("runtime/tasks/{0}/comments", entry.id));
                    entry.jvariables = GetWorkflowInstanceDetails(String.Format("runtime/tasks/{0}/variables", entry.id));
                    entry.jevents = GetWorkflowInstanceDetails(String.Format("runtime/tasks/{0}/events", entry.id));
                    entry.jattachments = GetWorkflowInstanceDetails(String.Format("runtime/tasks/{0}/attachments", entry.id));
                }
            }
            return result;
        }

        public string GetWorkflowInstanceDetails (string uri)
        {
            var response = _Client.Invoke(uri, _QueryParameters());
            return response;
        }

        #endregion GetWorkflowInstance(s) end

        #region GetWorkflow indepth informations...

        public T GetWorkflowIndepth<T>(string instanceId, EnumIndepth Indepth)
        {
            Contract.Requires(instanceId != null);

            var uri = string.Format("query/{0}", Indepth.ToString().ToLower());
            var request = new Hashtable();
            request.Add("processInstanceId", instanceId);
            var jrequest = JsonConvert.SerializeObject(request);

            Debug.WriteLine(string.Format("Body: {0}", jrequest));
            var response = _Client.Invoke("POST", uri, null, null, jrequest);

            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public ProcessExecutionsResponse GetWorkflowExections(string instanceId)
        {
            var result = GetWorkflowIndepth<ProcessExecutionsResponse>(instanceId, biz.dfch.CS.Activiti.Client.ProcessEngine.EnumIndepth.Executions);
            return result;
        }

        public ProcessTasksResponse GetWorkflowTasks(string instanceId)
        {
            var result = GetWorkflowIndepth<ProcessTasksResponse>(instanceId, biz.dfch.CS.Activiti.Client.ProcessEngine.EnumIndepth.Tasks);
            return result;
        }

        #endregion GetWorkflow indepth informations end

        #region UpdateWorkflowInstance

        public T UpdateWorkflowInstance<T>(string id, EnumStatus status)
        {
            Contract.Requires(id != null);

            var uri = string.Format("runtime/process-instances/{0}", id);
            var request = new ProcessInstanceRequestUpdateData()
            {
                action = status.ToString().ToLower()
            };
            var jrequest = JsonConvert.SerializeObject(request);

            Debug.WriteLine(string.Format("Body: {0}", jrequest));
            var response = _Client.Invoke("PUT", uri, null, null, jrequest);

            var result = JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public ProcessInstanceResponseData UpdateWorkflowInstance(string id, EnumStatus status)
        {
            var result = UpdateWorkflowInstance<ProcessInstanceResponseData>(id, status);
            result.variables = GetWorkflowInstanceVariables<List<ProcessVariableData>>(id);
            return result;
        }

        #endregion UpdateWorkflowInstance end

        #region DeleteWorkflowInstance

        public void DeleteWorkflowInstance(string id)
        {
            Contract.Requires(id != null);

            var uri = string.Format("runtime/process-instances/{0}", id);
            var response = _Client.Invoke("DELETE", uri, null, null, null);
        }

        #endregion DeleteWorkflowInstance end

        #endregion

    }
}
