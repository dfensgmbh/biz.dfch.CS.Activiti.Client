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

    public class ProcessExecutionResponseData
    {
        public string id { get; set; }
        public string url { get; set; }
        public string parentId { get; set; }
        public string parentUrl { get; set; }
        public bool suspended { get; set; }
        public string processInstanceId { get; set; }
        public string processInstanceUrl { get; set; }
        public string activityId { get; set; }
        public string tenantId { get; set; }

        public string jactivities { get; set; }
        public string jvariables { get; set; }

    }

}
