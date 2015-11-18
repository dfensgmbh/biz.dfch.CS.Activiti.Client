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

    #region activiti process-definitions v5.16.4.0
    public class ProcessDefinitionsResponseData
    {
        [Required]
        public string id { get; set; }
        [Required]
        public string url { get; set; }
        public int version { get; set; }
        public string key { get; set; }
        public string category { get; set; }
        public bool suspended { get; set; }
        [Required]
        public string name { get; set; }
        public string description { get; set; }
        public string deploymentId { get; set; }
        public string deploymentUrl { get; set; }
        public bool graphicalNotationDefined { get; set; }
        public string resource { get; set; }
        public string diagramResource { get; set; }
        public bool startFormDefined { get; set; }

    }
        
    public class ProcessDefinitionsResponse
    {
        public List<ProcessDefinitionsResponseData> data { get; set; }
        public int size { get; set; }
        public string order { get; set; }
        public string sort { get; set; }
        public int start { get; set; }
        public int total { get; set; }

    }
    #endregion

}
