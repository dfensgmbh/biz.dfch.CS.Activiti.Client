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
 
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Activiti.Client
{

    [ContractClassFor(typeof(IBPMService))]
    public abstract class ContractClassForIBPMService : IBPMService
    {
        public ContractClassForIBPMService(Uri server, string applicationName = "", int timeoutSec = 0)
        {
            Contract.Requires(server != null);
            Contract.Requires(!string.IsNullOrEmpty(server.Host), "Host is missing!");
            Contract.Requires(applicationName != null);
            Contract.Requires(timeoutSec >= 0);
        }

        #region methods

        public bool IsLoggedIn()
        {
            return false;
        }

        public void Login(string username, string password)
        {
            Contract.Requires(username != null);
            Contract.Requires(password != null);
        }

        public void Login(NetworkCredential credential)
        {
            Contract.Requires(credential != null);
        }

        public void Logout()
        {
            Contract.Requires(IsLoggedIn() == true);
            Contract.Assume(IsLoggedIn() == false);
        }

        public T GetWorkflowDefinitions<T>()
        {
            return default(T);
        }

        public object GetWorkflowDefinitions(Type type)
        {
            Contract.Requires(type != null);
            return default(object);
        }

        public object GetWorkflowDefinitions(object type)
        {
            Contract.Requires(type != null);
            return default(object);
        }

        public ProcessDefinitionsResponse GetWorkflowDefinitions()
        {
            return default(ProcessDefinitionsResponse);
        }

        public T InvokeWorkflowInstance<T>(string definitionId, List<ProcessVariableData> variables)
        {
            Contract.Requires(!string.IsNullOrEmpty(definitionId));
            Contract.Requires(variables != null);
            return default(T);
        }

        public T InvokeWorkflowTenantInstance<T>(string definitionId, List<ProcessVariableData> variables, string tenantId)
        {
            Contract.Requires(!string.IsNullOrEmpty(definitionId));
            Contract.Requires(variables != null);
            Contract.Requires(!string.IsNullOrEmpty(tenantId));
            return default(T);
        }

        public ProcessInstanceResponseData InvokeWorkflowInstance(string definitionId, Hashtable variablesHt, string tenantId)
        {
            Contract.Requires(!string.IsNullOrEmpty(definitionId));
            Contract.Requires(variablesHt != null);
            return default(ProcessInstanceResponseData);
        }

        public T GetWorkflowInstances<T>()
        {
            return default(T);
        }

        public ProcessInstancesResponse GetWorkflowInstances()
        {
            return default(ProcessInstancesResponse);
        }

        public T GetWorkflowInstanceVariables<T>(string id)
        {
            Contract.Requires(!string.IsNullOrEmpty(id));
            return default(T);
        }

        public T GetWorkflowInstance<T>(string id, bool completed)
        {
            Contract.Requires(!string.IsNullOrEmpty(id));
            return default(T);
        }

        public ProcessInstanceResponseData GetWorkflowInstance(string id)
        {
            Contract.Requires(!string.IsNullOrEmpty(id));
            return default(ProcessInstanceResponseData);
        }

        public ProcessInstanceResponseIndepthData GetWorkflowInstance(string id, bool indepth)
        {
            Contract.Requires(!string.IsNullOrEmpty(id));
            return default(ProcessInstanceResponseIndepthData);
        }

        public string Invoke(string uri)
        {
            Contract.Requires(!string.IsNullOrEmpty(uri));
            return default(string);
        }

        public T GetWorkflowIndepth<T>(string instanceId, biz.dfch.CS.Activiti.Client.ProcessEngine.EnumIndepth indepth)
        {
            Contract.Requires(!string.IsNullOrEmpty(instanceId));
            return default(T);
        }

        public ProcessExecutionsResponse GetWorkflowExecutions(string instanceId)
        {
            Contract.Requires(!string.IsNullOrEmpty(instanceId));
            return default(ProcessExecutionsResponse);
        }

        public ProcessTasksResponse GetWorkflowTasks(string instanceId)
        {
            Contract.Requires(!string.IsNullOrEmpty(instanceId));
            return null;
        }

        public T UpdateWorkflowInstance<T>(string id, biz.dfch.CS.Activiti.Client.ProcessEngine.EnumStatus status)
        {
            Contract.Requires(!string.IsNullOrEmpty(id));
            return default(T);
        }

        public ProcessInstanceResponseData UpdateWorkflowInstance(string id, biz.dfch.CS.Activiti.Client.ProcessEngine.EnumStatus status)
        {
            Contract.Requires(!string.IsNullOrEmpty(id));
            return default(ProcessInstanceResponseData);
        }

        public bool DeleteWorkflowInstance(string id)
        {
            Contract.Requires(!string.IsNullOrEmpty(id));
            return default(bool);
        }

        #endregion
    }
}
