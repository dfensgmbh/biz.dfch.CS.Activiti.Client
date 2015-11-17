﻿/**
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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Activiti.Client
{
    public class RestClient
    {
        // DFTODO - define properties such as Username, Password, Server, ...

        public Uri Server { get; set; }

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

        public RestClient()
        {
            // N/A
        }

        public RestClient(Uri server, string username, string password)
        {
            Contract.Requires(null != server);
            Contract.Requires(!string.IsNullOrWhiteSpace(username));
            Contract.Requires(!string.IsNullOrWhiteSpace(password));

            Server = server;
            Username = username;
            Password = password;
        }
    }
}