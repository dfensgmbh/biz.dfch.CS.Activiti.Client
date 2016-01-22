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
using System.IO;

namespace biz.dfch.CS.Activiti.Client
{
    public class ProcessEngine : IBPMService
    {
        #region private variables

        private RestClient _Client = null;
        private string _ApplicationName = "";
        private bool _IsLoggedIn = false;

        #endregion

        #region constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="server">Uri from the REST-Client</param>
        /// <param name="applicationName">Optional application name.</param>
        /// <param name="timeoutSec">Timeout REST-Client. Must be greater than 0. Else the clients default is used.</param>
        public ProcessEngine(Uri server, string applicationName = "", int timeoutSec = 0)
        {
            this._ApplicationName = applicationName;
            this._Client = new RestClient(server);
            if (timeoutSec > 0) this._Client.TimeoutSec = timeoutSec;
        }

        #endregion

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

        #region Methods

        #region Login and Logout

        public bool IsLoggedIn()
        {
            return _IsLoggedIn;
        }

        public void Login(string username, string password)
        {
            this._Client.SetCredential(username, password);
            Login();
        }

        public void Login(NetworkCredential credential)
        {
            this._Client.Credential = credential;
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
            if (!IsLoggedIn()) throw new Exception("Perform a login before logout.");
            this._Client.Credential = new NetworkCredential(String.Empty, String.Empty);
            this._IsLoggedIn = false;
        }

        #endregion Login and Logout end

        #region Deployment(s)

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetDeployments<T>(string name = "")
        {
            var uri = string.Format("repository/deployments");
            Hashtable ht = _QueryParameters();
            if (!string.IsNullOrEmpty(name)) ht.Add("name", name);
            var response = _Client.Invoke(uri, ht);

            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public object GetDeployments(Type type, string name = "")
        {
            Contract.Requires(type != null);

            var mi = this.GetType().GetMethods().Where(m => (m.Name == "GetDeployments" && m.IsGenericMethod)).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] {/*parameters*/ name });
            return result;
        }

        public object GetDeployments(object type, string name = "")
        {
            Contract.Requires(type != null);

            var mi = this.GetType().GetMethods().Where(m => (m.Name == "GetDeployments" && m.IsGenericMethod)).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] {/*parameters*/ name });
            return result;
        }

        public DeploymentResponse GetDeployments(string name = "")
        {
            var result = GetDeployments<DeploymentResponse>(name);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deploymentId"></param>
        /// <returns></returns>
        public T GetDeployment<T>(string deploymentId)
        {
            Contract.Requires(deploymentId != null);

            var uri = string.Format("repository/deployments/{0}", deploymentId);
            var response = _Client.Invoke(uri);

            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public DeploymentResponseData GetDeployment(string deploymentId)
        {
            var result = GetDeployment<DeploymentResponseData>(deploymentId);
            return result;
        }

        #endregion

        #region CreateDeployment

        private T CreateDeployment<T>(string filename, byte[] file)
        {
            Contract.Requires(file != null);

            var uri = string.Format("repository/deployments");

            var response = _Client.Upload(uri, file, "file", filename);
            var result = (T)JsonConvert.DeserializeObject<T>(response.Result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">HelloWorldProcess.bpmn20.xml</param>
        /// <param name="file">Bytes of the bpmn20.xml-File</param>
        /// <returns></returns>
        public DeploymentResponseData CreateDeployment(string filename, byte[] file)
        {
            filename = Path.GetFileName(filename); // Just filename. Without path.

            var result = CreateDeployment<DeploymentResponseData>(filename, file);
            return result;
        }

        public DeploymentResponseData CreateDeployment(string filePath, string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(filePath));
            Contract.Requires(System.IO.File.Exists(filePath));

            byte[] bytes = File.ReadAllBytes(filePath);

            var result = CreateDeployment<DeploymentResponseData>(name, bytes);
            return result;

        }



        #endregion

        #region DeleteDeployment

        public bool DeleteDeployment(string id)
        {
            Contract.Requires(id != null);

            var uri = string.Format("repository/deployments/{0}", id);
            try
            {
                var response = _Client.Invoke("DELETE", uri, null, null, null);
                return response == string.Empty;
            }
            catch (Exception e)
            {
                // If there are still running process instances on this deployment, it cannot be deleted. In Fiddler you ca see the following error:
                // {"message":"Internal server error","exception":"\n### Error updating database.  Cause: org.postgresql.util.PSQLException: ERROR: update or delete on table \"act_re_procdef\" violates foreign key constraint \"act_fk_exe_procdef\" on table \"act_ru_execution\"\n  Detail: Key (id_)=(exceptionAfterDurationProcessUnitTests:1:216775) is still referenced from table \"act_ru_execution\".\n### The error may involve org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity.deleteProcessDefinitionsByDeploymentId-Inline\n### The error occurred while setting parameters\n### SQL: delete from ACT_RE_PROCDEF where DEPLOYMENT_ID_ = ?\n### Cause: org.postgresql.util.PSQLException: ERROR: update or delete on table \"act_re_procdef\" violates foreign key constraint \"act_fk_exe_procdef\" on table \"act_ru_execution\"\n  Detail: Key (id_)=(exceptionAfterDurationProcessUnitTests:1:216775) is still referenced from table \"act_ru_execution\"."}
                return false;
            }

        }



        #endregion

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="definitionId">createTimersProcess:1:36</param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">createTimersProcess</param>
        /// <returns></returns>
        public ProcessDefinitionsResponse GetWorkflowDefinitionByKey(string key, bool latest = false)
        {
            Contract.Requires(key != null);

            var uri = string.Format("repository/process-definitions");
            Hashtable ht = _QueryParameters();
            ht.Add("key", key);
            if (latest) ht.Add("latest", "true");
            var response = _Client.Invoke(uri, ht);

            var result = (ProcessDefinitionsResponse)JsonConvert.DeserializeObject<ProcessDefinitionsResponse>(response);
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
            string jrequest = JsonConvert.SerializeObject(request);

            Debug.WriteLine(string.Format("Body: {0}", jrequest));
            var response = _Client.Invoke("POST", uri, null, null, jrequest);
            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public T InvokeWorkflowTenantInstance<T>(string definitionId, List<ProcessVariableData> variables, string tenantId)
        {
            Contract.Requires(definitionId != null);

            var uri = string.Format("runtime/process-instances");
            var request = new ProcessInstanceRequestTenantData()
            {
                processDefinitionKey = definitionId,
                businessKey = _ApplicationName,
                tenantId = tenantId
            };
            request.variables = variables;
            string jrequest = JsonConvert.SerializeObject(request);

            Debug.WriteLine(string.Format("Body: {0}", jrequest));
            var response = _Client.Invoke("POST", uri, null, null, jrequest);
            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public ProcessInstanceResponseData InvokeWorkflowInstance(string definitionId, Hashtable variablesHt, string tenantId = "")
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
            if (tenantId == "")
            {
                return InvokeWorkflowInstance<ProcessInstanceResponseData>(definitionId, variables);
            }
            else
            {
                return InvokeWorkflowTenantInstance<ProcessInstanceResponseData>(definitionId, variables, tenantId);
            }
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

        public T GetWorkflowInstanceVariables<T>(string id, bool completed)
        {
            Contract.Requires(id != null);

            var uri = completed ? string.Format("history/historic-variable-instances?processInstanceId={0}", id) : string.Format("runtime/process-instances/{0}/variables", id);
            var response = _Client.Invoke(uri);
            //var response = _Client.Invoke(uri, _QueryParameters());

            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public T GetWorkflowInstance<T>(string id, bool completed)
        {
            Contract.Requires(id != null);

            var uri = completed ? string.Format("history/historic-process-instances/{0}", id) : string.Format("runtime/process-instances/{0}", id);
            var response = _Client.Invoke(uri);

            var result = (T)JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public ProcessInstanceResponseData GetWorkflowInstance(string id)
        {
            ProcessInstanceResponseData result;
            try
            {
                result = GetWorkflowInstance<ProcessInstanceResponseData>(id, false);
            }
            catch (Exception e)
            {
                result = GetWorkflowInstance<ProcessInstanceResponseData>(id, true);
                result.completed = true;
                result.ended = true;
                result.suspended = false;
            }

            return result;
        }

        public ProcessInstanceResponseIndepthData GetWorkflowInstance(string id, bool indepth)
        {
            ProcessInstanceResponseIndepthData result;
            try
            {
                result = GetWorkflowInstance<ProcessInstanceResponseIndepthData>(id, false);
            }
            catch (Exception)
            {
                result = GetWorkflowInstance<ProcessInstanceResponseIndepthData>(id, true);
                result.completed = true;
                result.ended = true;
                result.suspended = false;
                ProcessHistoricVariableInstancesResponse historicVariableInstances = GetWorkflowInstanceVariables<ProcessHistoricVariableInstancesResponse>(id, true);
                var variables = new List<ProcessVariableData>();
                foreach (ProcessHistoricInstancesResponse historicVariableInstance in historicVariableInstances.data)
                {
                    variables.Add(new ProcessVariableData()
                    {
                        name = historicVariableInstance.variable.name.ToString(),
                        value = historicVariableInstance.variable.value.ToString()
                    }
                    );
                }
                result.variables = variables;
                indepth = false;
            }

            if (indepth == true)
            {
                // get variables
                result.variables = GetWorkflowInstanceVariables<List<ProcessVariableData>>(id, false);
                // get executions
                var executions = GetWorkflowIndepth<ProcessExecutionsResponse>(id, biz.dfch.CS.Activiti.Client.ProcessEngine.EnumIndepth.Executions);
                result.executions = executions.data;
                foreach (var entry in result.executions)
                {
                    // get execution indepth details
                    entry.jactivities = Invoke(String.Format("runtime/executions/{0}/activities", entry.id));
                    //entry.jvariables = GetWorkflowInstanceDetails(String.Format("runtime/executions/{0}/variables", entry.id));
                }
                // get tasks
                var tasks = GetWorkflowIndepth<ProcessTasksResponse>(id, biz.dfch.CS.Activiti.Client.ProcessEngine.EnumIndepth.Tasks);
                result.tasks = tasks.data;
                foreach (var entry in result.tasks)
                {
                    //history/historic-process-instances
                    //history/historic-task-instances
                    // get task indepth details
                    entry.jidentitylinks = Invoke(String.Format("runtime/tasks/{0}/identitylinks", entry.id));
                    entry.jcomments = Invoke(String.Format("runtime/tasks/{0}/comments", entry.id));
                    entry.jvariables = Invoke(String.Format("runtime/tasks/{0}/variables", entry.id));
                    entry.jevents = Invoke(String.Format("runtime/tasks/{0}/events", entry.id));
                    entry.jattachments = Invoke(String.Format("runtime/tasks/{0}/attachments", entry.id));
                }
            }
            return result;
        }

        public string Invoke(string uri)
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

        public ProcessExecutionsResponse GetWorkflowExecutions(string instanceId)
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
            result.variables = GetWorkflowInstanceVariables<List<ProcessVariableData>>(id, false);
            return result;
        }

        #endregion UpdateWorkflowInstance end

        #region DeleteWorkflowInstance

        public bool DeleteWorkflowInstance(string id)
        {
            Contract.Requires(id != null);

            var uri = string.Format("runtime/process-instances/{0}", id);
            try
            {
                var response = _Client.Invoke("DELETE", uri, null, null, null);
                return response == string.Empty;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        #endregion DeleteWorkflowInstance end

        #endregion
    }
}
