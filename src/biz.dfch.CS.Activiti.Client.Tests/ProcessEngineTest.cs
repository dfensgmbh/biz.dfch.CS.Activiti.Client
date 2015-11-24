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
using System.Net.Sockets;
using System.Net;
using System.Diagnostics.Contracts;
using Telerik.JustMock;
using System.Collections;
using System.Net.Http;

namespace biz.dfch.CS.Activiti.Client.Tests
{
    [TestClass]
    public class ProcessEngineTest
    {
        #region test initialisation and cleanup

        protected Uri serveruri = new Uri("http://192.168.112.129:9000/activiti-rest/service/");
        protected string applicationName = "";
        protected string username = "kermit";
        protected string password = "kermit";
        protected ProcessEngine _ProcessEngine = null;

        [TestInitialize]
        public void TestInitialize()
        {
            _ProcessEngine = new ProcessEngine(serveruri, applicationName);
        }

        [TestCleanup]
        public void TestCleanup()
        {

        }

        #endregion

        #region test methods

        [TestMethod]
        public void Test()
        {

        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        [ExpectedException(typeof(AggregateException), "Invalid uri.")]
        public void LoginWithInvalidUri()
        {
            ProcessEngine ProcessEngine = new ProcessEngine(new Uri("http://localhost:9000/activiti-rest/service/"), "InvalidClient");
            ProcessEngine.Login("wrongusername", "1234");
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        [ExpectedException(typeof(UnauthorizedAccessException), "A wrong username was inappropriately allowed.")]
        public void LoginWithWrongUsernameAndPassword()
        {
            this._ProcessEngine.Login("wrongusername", "1234");
        }


        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void Login()
        {
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());
        }


        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void LogoutFailed()
        {
            try
            {
                this._ProcessEngine.Logout();
            }
            catch (Exception)
            {

            }
            
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void Logout()
        {
            this._ProcessEngine.Login(username, password);
            this._ProcessEngine.Logout();
            Assert.IsFalse(this._ProcessEngine.IsLoggedIn());
        }


        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetWorkflowDefinitions()
        {
            // Arrange


            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());
            var wdefObj1 = this._ProcessEngine.GetWorkflowDefinitions<ProcessDefinitionsResponse>();
            var wdefObj2 = this._ProcessEngine.GetWorkflowDefinitions();

            // Assert
            Assert.IsNotNull(wdefObj1);
            Assert.IsTrue(wdefObj1.total > 0);
            Assert.IsNotNull(wdefObj2);
            Assert.IsTrue(wdefObj2.total > 0);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetWorkflowDefinitionsEmpty()
        {
            // Arrange


            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());
            Mock.Arrange(() => _ProcessEngine.GetWorkflowDefinitions<ProcessDefinitionsResponse>()).Returns(new ProcessDefinitionsResponse() { total = 0 });
            var wdefObj = this._ProcessEngine.GetWorkflowDefinitions();

            // Assert
            Assert.IsNotNull(wdefObj);
            Assert.IsTrue(wdefObj.total == 0);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void InvokeWorkflow()
        {
            // Arrange
            var definitionid = "createTimersProcess:1:31";
            var vars = new Hashtable();
            vars.Add("duration", "long");
            vars.Add("throwException", "true");

            // Act
            this._ProcessEngine.Login(username, password);
            var instanceNew = this._ProcessEngine.InvokeWorkflowInstance(definitionid, vars);

            // Assert
            Assert.IsNotNull(instanceNew);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        [ExpectedException(typeof(ArgumentException), "Unknown workflow process definition")]
        public void InvokeWorkflowFail()
        {
            // Arrange
            var definitionid = "createTimersProcess:1:30";
            var vars = new Hashtable();
            vars.Add("duration", "long");
            vars.Add("throwException", "true");

            // Act
            this._ProcessEngine.Login(username, password);
            var instanceNew = this._ProcessEngine.InvokeWorkflowInstance(definitionid, vars);

            // Assert
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetWorkflowStatus()
        {
            // Arrange

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            var instances = _ProcessEngine.GetWorkflowInstances();
            var id = instances.data[0].id;
            var instance = _ProcessEngine.GetWorkflowInstance(id);

            // Assert
            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.completed);
            Assert.IsNotNull(instance.suspended);
            Assert.IsNotNull(instance.ended);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetWorkflowStatusFail()
        {
            // Arrange
            var id = "1234";

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            try
            {
                var instance = _ProcessEngine.GetWorkflowInstance(id);
            }
            catch (HttpRequestException httpE)
            {
                Assert.IsTrue(httpE.Message.Contains("404 (Not Found)."));
            }
            catch (Exception e)
            {

                throw;
            }

            // Assert
        }

        #endregion
    }
}
