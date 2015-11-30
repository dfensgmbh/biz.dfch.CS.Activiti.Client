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
using System.Linq;

namespace biz.dfch.CS.Activiti.Client.Tests
{
    [TestClass]
    public class ProcessEngineTest
    {

        #region constants

        const string DEFINITIONKEY_CREATETIMERSPROCESS = "createTimersProcess";
        const string DEFINITIONKEY_WILLFAIL = "WillFail";

        #endregion

        #region test initialisation and cleanup

        // DFTODO - move to app.config
        protected Uri serveruri = new Uri("http://172.19.115.38:9080/activiti-rest/service"); // "http://192.168.112.129:9000/activiti-rest/service/");
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
        [TestCategory("SkipOnTeamCity")]
        [ExpectedException(typeof(AggregateException), "Invalid uri.")]
        public void LoginWithInvalidUri()
        {
            ProcessEngine ProcessEngine = new ProcessEngine(new Uri("http://www.example.com/invalid-uri"), "InvalidClient");
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
                Assert.Fail();
            }
            catch (Exception)
            {
                // TEst must throw an exception
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
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "long");
            vars.Add("throwException", "true");

            // Act

            this._ProcessEngine.Login(username, password);
            definitionid = GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);
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
            var definitionid = "invalid-definitionid:1:30";
            var vars = new Hashtable();
            vars.Add("duration", "long");
            vars.Add("throwException", "true");

            // Act
            this._ProcessEngine.Login(username, password);
            var instanceNew = this._ProcessEngine.InvokeWorkflowInstance(definitionid, vars);

            // Assert
            Assert.Fail("Test should have failed before.");
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

        #region Feature: Get Return of invoked (and completed) Workflow #7

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetWorkflowResultFromCompletedWorkflow()
        {
            // Arrange
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "short"); // short=10 seconds?
            vars.Add("throwException", "false");

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);
            System.Threading.Thread.Sleep(15000);
            ProcessInstanceResponseData instance = _ProcessEngine.GetWorkflowInstance(response.id);

            // Assert
            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.completed);
            Assert.IsTrue(instance.completed);
            Assert.IsNotNull(instance.suspended);
            Assert.IsNotNull(instance.ended);


        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetWorkflowResultFromEndedWorkflow()
        {
            // Arrange
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "short"); // short=10 seconds?
            vars.Add("throwException", "false");

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);
            System.Threading.Thread.Sleep(15000);
            ProcessInstanceResponseData instance = _ProcessEngine.GetWorkflowInstance(response.id);

            // Assert
            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.completed);
            Assert.IsNotNull(instance.suspended);
            Assert.IsNotNull(instance.ended);
            Assert.IsTrue(instance.ended);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetWorkflowResultFromFailedWorkflow()
        {
            // Arrange
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "short"); // short=10 seconds?
            vars.Add("throwException", "true");
            //string expectedReturnVariable = "returnVariableName from definitionid-workflow"; // TODO: Use a workflow which returns a result
            //string expectedReturnValue = "expected value"; // TODO: Expected value

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);
            System.Threading.Thread.Sleep(11000);
            ProcessInstanceResponseData instance = _ProcessEngine.GetWorkflowInstance(response.id);
            //ProcessVariableData variable = instance.variables.Where(v => v.name == expectedReturnVariable).FirstOrDefault();

            // Assert
            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.completed);
            Assert.IsNotNull(instance.suspended);
            Assert.IsNotNull(instance.ended);
            Assert.IsTrue(instance.ended);

            // TODO: What is the state of a failed workflow? throwing an exception means failed?

            //Assert.IsNotNull(variable);
            //Assert.IsTrue(variable.value == expectedReturnValue);
        }


        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetWorkflowResultFromSuspendedWorkflow()
        {
            // Arrange
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "short"); // short=10 seconds?
            vars.Add("throwException", "true");
            //string expectedReturnVariable = "returnVariableName from definitionid-workflow"; // TODO: Use a workflow which returns a result
            //string expectedReturnValue = "expected value"; // TODO: Expected value

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);

            ProcessInstanceResponseData suspended = _ProcessEngine.UpdateWorkflowInstance(response.id, ProcessEngine.EnumStatus.Suspend);

            ProcessInstanceResponseData instance = _ProcessEngine.GetWorkflowInstance(response.id);
            //ProcessVariableData variable = instance.variables.Where(v => v.name == expectedReturnVariable).FirstOrDefault();

            // Assert
            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.completed);
            Assert.IsNotNull(instance.suspended);
            Assert.IsNotNull(instance.ended);
            Assert.IsTrue(instance.ended);

            // TODO: What is the state of a failed workflow? throwing an exception means failed?

            //Assert.IsNotNull(variable);
            //Assert.IsTrue(variable.value == expectedReturnValue);
        }


        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetWorkflowResultFromRunningWorkflowFail()
        {
            // Arrange
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "short"); // short=10 seconds?
            vars.Add("throwException", "true");
            //string expectedReturnVariable = "returnVariableName from definitionid-workflow"; // TODO: Use a workflow which returns a result
            //string expectedReturnValue = "expected value"; // TODO: Expected value

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);
            ProcessInstanceResponseData instance = _ProcessEngine.GetWorkflowInstance(response.id);

            // Assert
            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.completed);
            Assert.IsNotNull(instance.suspended);
            Assert.IsNotNull(instance.ended);
            Assert.IsFalse(instance.completed);
            Assert.IsFalse(instance.suspended);
            Assert.IsFalse(instance.ended);


            // TODO: What is the state of a failed workflow? throwing an exception means failed?

            //Assert.IsNotNull(variable);
            //Assert.IsTrue(variable.value == expectedReturnValue);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        [ExpectedException(typeof(NotSupportedException))]
        public void GetWorkflowResultWithFalseWorkflowId()
        {
            // Arrange
            string unknownId = "0";

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            ProcessInstanceResponseData instance = _ProcessEngine.GetWorkflowInstance(unknownId);
            if (instance == null) throw new NotSupportedException("No workflow exists.");

            // Assert

        }

        #endregion


        #region "Feature: Cancel Invoked Workflow #6"

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CancelRunningWorkflow()
        {
            // Arrange
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "short"); // short=10 seconds?
            vars.Add("throwException", "true");

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);
            bool endedBeforeCanceling = response.ended;
            bool cancelled = _ProcessEngine.DeleteWorkflowInstance(response.id);

            // Assert

            Assert.IsTrue(cancelled);

        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CancelSuspendedWorkflow()
        {
            // Arrange
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "short"); // short=10 seconds?
            vars.Add("throwException", "true");

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);

            ProcessInstanceResponseData suspended = _ProcessEngine.UpdateWorkflowInstance(response.id, ProcessEngine.EnumStatus.Suspend);

            bool suspendedBeforeCanceling = suspended.suspended;
            bool endedBeforeCanceling = suspended.ended;
            bool cancelled = _ProcessEngine.DeleteWorkflowInstance(response.id);

            // Assert
            Assert.IsTrue(suspendedBeforeCanceling);
            Assert.IsFalse(endedBeforeCanceling);
            Assert.IsTrue(cancelled);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CancelCompletedWorkflowFail()
        {
            // Arrange
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "short"); // short=10 seconds?
            vars.Add("throwException", "false");

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);
            System.Threading.Thread.Sleep(15000); // Wait, till precess finished...

            ProcessInstanceResponseData instance = _ProcessEngine.GetWorkflowInstance(response.id);
            bool completedBeforeCanceling = instance.completed;
            bool cancelled = _ProcessEngine.DeleteWorkflowInstance(response.id);

            // Assert
            Assert.IsTrue(completedBeforeCanceling);
            Assert.IsFalse(cancelled);
        }


        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CancelFailedWorkflowFail()
        {
            // Arrange
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "short"); // short=10 seconds?
            vars.Add("throwException", "true");

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_WILLFAIL);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);

            System.Threading.Thread.Sleep(15000); // Wait, till precess finished...

            ProcessInstanceResponseData instance = _ProcessEngine.GetWorkflowInstance(response.id);
            bool failedBeforeCanceling = !instance.completed && instance.ended; // TODO: What is the state of failed!? response.failed

            Assert.IsTrue(failedBeforeCanceling);

            bool cancelled = _ProcessEngine.DeleteWorkflowInstance(response.id);


            // Assert
            Assert.IsFalse(cancelled);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CancelEndedWorkflowFail()
        {
            // Arrange
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "short"); // short=10 seconds?
            vars.Add("throwException", "false");

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);
            System.Threading.Thread.Sleep(15000);

            ProcessInstanceResponseData instance = _ProcessEngine.GetWorkflowInstance(response.id);
            bool endedBeforeCanceling = instance.ended;

            Assert.IsTrue(endedBeforeCanceling);

            bool cancelled = _ProcessEngine.DeleteWorkflowInstance(response.id);


            // Assert
            Assert.IsFalse(cancelled);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CancelNonExistingWorkflowFail()
        {
            // Arranged
            string unknownId = "0";

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            bool deleted = _ProcessEngine.DeleteWorkflowInstance(unknownId);
            // Assert

            Assert.IsFalse(deleted);
        }

        #endregion

        [TestMethod]
        public void TestMethod()
        {
            
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Returns the definitionid of a given definitionkey.
        /// </summary>
        /// <param name="definitionkey">Something like createTimersProcess (see const  DEFINITIONKEY_CREATETIMERSPROCESS)</param>
        /// <returns> Something like "createTimersProcess:1:31" (version can change)</returns>
        private string GetDefinitionId(string definitionkey)
        {
            ProcessDefinitionsResponse definitions = this._ProcessEngine.GetWorkflowDefinitions();

            return definitions.data.Where(d => d.key == definitionkey).FirstOrDefault().id;
        }

        #endregion
    }
}
