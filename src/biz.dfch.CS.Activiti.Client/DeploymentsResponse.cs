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

    //            {
    //  "data": [
    //    {
    //      "id": "160428",
    //      "name": "xy-cms-workflows-1.0-SNAPSHOT.bar",
    //      "deploymentTime": "2016-01-19T11:33:19.004+01:00",
    //      "category": null,
    //      "url": "http://172.19.115.38:9080/activiti-rest/service/repository/deployments/111111",
    //      "tenantId": ""
    //    },
    //    {
    //      "id": "162612",
    //      "name": "FinancialReportProcess2.bpmn20.xml",
    //      "deploymentTime": "2016-01-19T13:07:28.843+01:00",
    //      "category": null,
    //      "url": "http://172.19.115.38:9080/activiti-rest/service/repository/deployments/22222",
    //      "tenantId": ""
    //    }
    //  ],
    //  "total": 2,
    //  "start": 0,
    //  "sort": "id",
    //  "order": "asc",
    //  "size": 2
    //}

    #endregion

    public class DeploymentResponse
    {
        public List<DeploymentResponseData> data { get; set; }
        public int size { get; set; }
        public string order { get; set; }
        public string sort { get; set; }
        public int start { get; set; }
        public int total { get; set; }

    }

}
