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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Activiti.Client
{
    [ContractClass(typeof(ContractClassForIBPMService))]
    interface IBPMService
    {
        bool IsLoggedIn();

        void Login(string username, string password);

        void Login(NetworkCredential credential);

        void Logout();

        T GetWorkflowDefinitions<T>();

        object GetWorkflowDefinitions(Type type);

        object GetWorkflowDefinitions(object type);

        ProcessDefinitionsResponse GetWorkflowDefinitions();

        T InvokeWorkflowInstance<T>(string definitionId, List<ProcessVariableData> variables);

        T InvokeWorkflowTenantInstance<T>(string definitionId, List<ProcessVariableData> variables, string tenantId);

        ProcessInstanceResponseData InvokeWorkflowInstance(string definitionId, Hashtable variablesHt, string tenantId);

        T GetWorkflowInstances<T>();

        ProcessInstancesResponse GetWorkflowInstances();

        T GetWorkflowInstanceVariables<T>(string id);

        T GetWorkflowInstance<T>(string id, bool completed);

        ProcessInstanceResponseData GetWorkflowInstance(string id);

        ProcessInstanceResponseIndepthData GetWorkflowInstance(string id, bool indepth);

        string Invoke(string uri);

        T GetWorkflowIndepth<T>(string instanceId, biz.dfch.CS.Activiti.Client.ProcessEngine.EnumIndepth Indepth);

        ProcessExecutionsResponse GetWorkflowExecutions(string instanceId);

        ProcessTasksResponse GetWorkflowTasks(string instanceId);

        T UpdateWorkflowInstance<T>(string id, biz.dfch.CS.Activiti.Client.ProcessEngine.EnumStatus status);

        ProcessInstanceResponseData UpdateWorkflowInstance(string id, biz.dfch.CS.Activiti.Client.ProcessEngine.EnumStatus status);

        bool DeleteWorkflowInstance(string id);

    }
}
