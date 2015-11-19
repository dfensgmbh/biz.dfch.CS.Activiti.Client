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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections;

namespace biz.dfch.CS.Activiti.Client.Tests
{
    [TestClass]
    public class SampleTest
    {
        // Arrange
        protected Uri serveruri = new Uri("http://192.168.112.129:9000/activiti-rest/service/");
        protected string username = "kermit";
        protected string password = "kermit";
        protected string applicationName = "ProcessEngine#1";
        protected ProcessEngine processEngine = null;

        [TestInitialize]
        public void TestInitialize()
        {
            processEngine = new ProcessEngine(serveruri, applicationName);
        }

        [TestMethod]
        public void SampleTests()
        {
            // Arrange

            // Act
            processEngine.Login(username, password);

            /// Get all definitions
            var workflows = processEngine.GetWorkflowDefinitions();

            /// Get a definition
            var definitionid = "createTimersProcess:1:31";
            var workflow = processEngine.GetWorkflowDefinition(definitionid);

            /// Create a instance
            var vars = new Hashtable();
            vars.Add("duration", "long");
            vars.Add("throwException", "true");            
            var instanceNew = processEngine.InvokeWorkflowInstance(definitionid, vars);

            /// Get instances
            var id = instanceNew.id;
            var instances = processEngine.GetWorkflowInstances();
            var instance = processEngine.GetWorkflowInstance(id);            
            var instanceIndepth = processEngine.GetWorkflowInstance(id, true);

            /// Update instance
            var instanceUpdate = processEngine.UpdateWorkflowInstance(id, biz.dfch.CS.Activiti.Client.ProcessEngine.EnumStatus.Suspend);

            /// Delete instance
            processEngine.DeleteWorkflowInstance(id);
            
            // Assert
            Assert.IsNotNull(processEngine);
            Assert.IsNotNull(workflows);
            Assert.IsNotNull(workflow);
            Assert.IsNotNull(instanceNew);
            Assert.IsNotNull(instances);
            Assert.IsNotNull(instance);
            Assert.IsNotNull(instanceIndepth);
            Assert.IsNotNull(instanceUpdate);
        }

    }
}
