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
using System.ComponentModel.DataAnnotations;

namespace biz.dfch.CS.Activiti.Client
{

    public class ProcessInstanceResponseData
    {
        [Required]
        public string id { get; set; }
        [Required]
        public string url { get; set; }
        public string businessKey { get; set; }        
        public bool suspended { get; set; }
        public bool ended { get; set; }
        public bool completed { get; set; }
        [Required]
        public string processDefinitionId { get; set; }
        public string processDefinitionUrl { get; set; }
        public string activityId { get; set; }
        public string tenantId { get; set; }
        public List<ProcessVariableData> variables { get; set; }

    }

}
