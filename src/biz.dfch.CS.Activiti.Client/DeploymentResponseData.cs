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

    #region response example

    //    {
    //      "id": "162612",
    //      "name": "FinancialReportProcess2.bpmn20.xml",
    //      "deploymentTime": "2016-01-19T13:07:28.843+01:00",
    //      "category": null,
    //      "url": "http://172.19.115.38:9080/activiti-rest/service/repository/deployments/22222",
    //      "tenantId": ""
    //    }

    #endregion

    public class DeploymentResponseData
    {
        [Required]
        public string id { get; set; }
        [Required]
        public string url { get; set; }
        [Required]
        public string name { get; set; }
        public DateTime deploymentTime { get; set; }
        public string category { get; set; }
        public string tenantId { get; set; }
    }
}
