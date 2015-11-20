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
        #region protected variables

        protected RestClient _Client = null;
        protected string _ApplicationName = "";
        protected bool _IsLoggedIn = false;

        #endregion

        #region constructors

        public ContractClassForIBPMService()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="server">Uri from the REST-Client</param>
        /// <param name="applicationName">Optional application name.</param>
        /// <param name="timeoutSec">Timeout REST-Client. Must be greater than 0. Else the clients default is used.</param>
        public ContractClassForIBPMService(Uri server, string applicationName= "", int timeoutSec = 0)
        {
            Contract.Requires(server != null);
            this._ApplicationName = applicationName;
            this._Client = new RestClient(server);
            if (timeoutSec>0)this._Client.TimeoutSec = timeoutSec;

            Contract.Ensures(_Client != null);
        }

        #endregion

        #region properties

        public bool IsLoggedIn
        {
            get
            {
                return this._IsLoggedIn;
            }
        }

        #endregion

        #region methods

        public void Login(string username, string password)
        {
            Contract.Requires(_Client != null);
            Contract.Requires(username != null);
            Contract.Requires(password != null);

            this._Client.SetCredential(username, password);
        }

        public void Login(NetworkCredential credential)
        {
            Contract.Requires(credential != null);
            this._Client.Credential = credential;

        }

        public void Logout()
        {
            Contract.Requires(_Client != null);
        }

        public T GetWorkflowDefinitions<T>()
        {
            Contract.Requires(_Client != null);
            Contract.Requires(_IsLoggedIn);
            return default(T);

        }

        public object GetWorkflowDefinitions(Type type)
        {
            Contract.Requires(_Client != null);
            Contract.Requires(_IsLoggedIn);
            return default(object);
        }

        public object GetWorkflowDefinitions(object type)
        {
            Contract.Requires(_Client != null);
            Contract.Requires(_IsLoggedIn);
            return default(object);
        }

        public object GetWorkflowDefinitions()
        {
            Contract.Requires(_Client != null);
            Contract.Requires(_IsLoggedIn);
            return default(object);
        }

        public T InvokeWorkflowInstance<T>(string definitionId, List<ProcessVariableData> variables)
        {
            Contract.Requires(_Client != null);
            Contract.Requires(_IsLoggedIn);
            return default(T);
        }

        public object InvokeWorkflowInstance(string definitionId, Hashtable variablesHt)
        {
            Contract.Requires(_Client != null);
            Contract.Requires(_IsLoggedIn);
            return default(object);
        }

        public T GetWorkflowInstances<T>()
        {
            Contract.Requires(_Client != null);
            Contract.Requires(_IsLoggedIn);
            return default(T);
        }

        public object GetWorkflowInstances()
        {
            Contract.Requires(_Client != null);
            Contract.Requires(_IsLoggedIn);
            return default(object);
        }

        public object GetWorkflowInstance(string id)
        {
            Contract.Requires(_Client != null);
            Contract.Requires(_IsLoggedIn);
            return default(object);
        }

        public object UpdateWorkflowInstance(string id)
        {
            Contract.Requires(_Client != null);
            Contract.Requires(_IsLoggedIn);
            return default(object);
        }

        #endregion
    }
}
