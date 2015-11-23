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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace biz.dfch.CS.Activiti.Client
{

    public class ProcessTaskResponseData
    {
        public string assignee { get; set; }
        public string createTime { get; set; }
        public string delegationState { get; set; }
        public string description { get; set; }
        public bool suspended { get; set; }
        public string dueDate { get; set; }
        public string execution { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string owner { get; set; }
        public string parentTask { get; set; }
        public int priority { get; set; }
        public string processDefinition { get; set; }
        public string processInstance { get; set; }
        public string taskDefinitionKey { get; set; }
        public string url { get; set; }
        public string tenantId { get; set; }

        public string jidentitylinks { get; set; }
        public string jcomments { get; set; }
        public string jvariables { get; set; }
        public string jevents { get; set; }
        public string jattachments { get; set; }

    }

}
