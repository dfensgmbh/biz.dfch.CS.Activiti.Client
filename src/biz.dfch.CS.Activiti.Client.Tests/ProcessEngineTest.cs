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
using System.IO;

namespace biz.dfch.CS.Activiti.Client.Tests
{
    [TestClass]
    public class ProcessEngineTest
    {

        #region constants

        // This process definition is cerated in the unit tests if it does not exist and removed at the end.
        const string DEFINITIONKEY_CREATETIMERSPROCESS = "createTimersProcessUnitTests";
        const string DEFINITIONKEY_EXCEPTIONAFTERDURATIONSPROCESS = "exceptionAfterDurationProcessUnitTests";

        const int WAIT_TIMEOUT_MILLISECONDS = 30000;

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
            if (!this._ProcessEngine.IsLoggedIn())
            {
                this._ProcessEngine.Login(username, password);
            }



            foreach (string f in Directory.GetFiles("Resources"))
            {
                string fileName = Path.GetFileName(f);

                DeploymentResponse deployments = this._ProcessEngine.GetDeployments(fileName);

                // Deployments löschen
                foreach (DeploymentResponseData d in deployments.data.Where(d => d.name == fileName))
                {
                    if (d != null)
                    {
                        // If there are still running process instances, the deployment cannot be deleted.
                        bool deleted = this._ProcessEngine.DeleteDeployment(d.id);
                    }
                }
            }
        }

        #endregion

        #region test methods

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void LoginWithInvalidUri()
        {
            ProcessEngine processEngine = new ProcessEngine(new Uri("http://www.example.com/invalid-uri"), "InvalidClient");
            try
            {
                processEngine.Login("wrongusername", "1234");
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(processEngine);
                Assert.IsTrue(ex.Message == "Response status code does not indicate success: 404 (Not Found).");
            }
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
        public void GetDeployments()
        {
            // Arrange

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());
            DeploymentResponse response = this._ProcessEngine.GetDeployments();

            // Assert
            Assert.IsNotNull(response);

        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CreateDeploymentWithByteArray()
        {
            // Arrange

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            string filename = @"Resources\HelloWorldBProcess.bpmn20.xml";

            byte[] bytes = File.ReadAllBytes(filename);
            DeploymentResponseData response = this._ProcessEngine.CreateDeployment(filename, bytes);

            // Assert
            Assert.IsNotNull(response);
            int id = 0;
            Assert.IsTrue(int.TryParse(response.id, out id));
            Assert.IsTrue(id > 0);
            Assert.IsTrue(response.name == Path.GetFileName(filename));

            // Delete it
            bool deleted = this._ProcessEngine.DeleteDeployment(response.id);
            Assert.IsTrue(deleted);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CreateDeploymentWithFilename()
        {
            // Arrange

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            try
            {
                string filename = @"Resources\HelloWorldAProcess.bpmn20.xml";

                DeploymentResponseData response = this._ProcessEngine.CreateDeployment(filename, Path.GetFileName(filename));

                // Assert
                Assert.IsNotNull(response);
                int id = 0;
                Assert.IsTrue(int.TryParse(response.id, out id));
                Assert.IsTrue(id > 0);
                Assert.IsTrue(response.name == Path.GetFileName(filename));

                // Delete it
                bool deleted = this._ProcessEngine.DeleteDeployment(response.id);
                Assert.IsTrue(deleted);
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.ToString());
            }
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CreateDeploymentWithFilenameSleepAMinute()
        {
            // Arrange

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            try
            {
                string filename = @"Resources\SleepAMinuteProcess.bpmn20.xml";

                DeploymentResponseData response = this._ProcessEngine.CreateDeployment(filename, Path.GetFileName(filename));

                // Assert
                Assert.IsNotNull(response);
                int id = 0;
                Assert.IsTrue(int.TryParse(response.id, out id));
                Assert.IsTrue(id > 0);
                Assert.IsTrue(response.name == Path.GetFileName(filename));

                // Delete it
                bool deleted = this._ProcessEngine.DeleteDeployment(response.id);
                Assert.IsTrue(deleted);
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.ToString());
            }
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CreateDeploymentWithZIPFilename()
        {
            // Arrange

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            try
            {
                string zipFileName = @"Resources\zipFile.zip";

                DeploymentResponseData response = this._ProcessEngine.CreateDeployment(zipFileName, Path.GetFileName(zipFileName));

                // Assert
                Assert.IsNotNull(response);
                int id = 0;
                Assert.IsTrue(int.TryParse(response.id, out id));
                Assert.IsTrue(id > 0);
                Assert.IsTrue(response.name == Path.GetFileName(zipFileName));

                // Delete it
                bool deleted = this._ProcessEngine.DeleteDeployment(response.id);
                Assert.IsTrue(deleted);
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.ToString());
            }
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void DeleteDeployment()
        {
            // Arrange

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            string filename = @"Resources\HelloWorldAProcess.bpmn20.xml";

            byte[] bytes = File.ReadAllBytes(filename);
            DeploymentResponseData response = this._ProcessEngine.CreateDeployment(filename, bytes);

            // Assert
            Assert.IsNotNull(response);
            int id = 0;
            Assert.IsTrue(int.TryParse(response.id, out id));
            Assert.IsTrue(id > 0);
            Assert.IsTrue(response.name == Path.GetFileName(filename));

            bool deleted = this._ProcessEngine.DeleteDeployment(response.id);
            Assert.IsTrue(deleted);
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
        public void GetWorkflowDefinitionById()
        {
            // Arrange


            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());
            ProcessDefinitionsResponse definitions = this._ProcessEngine.GetWorkflowDefinitions();
            ProcessDefinitionResponseData def1 = definitions.data.FirstOrDefault();
            ProcessDefinitionResponseData def2 = this._ProcessEngine.GetWorkflowDefinition(def1.id);

            // Assert
            Assert.IsNotNull(def1);
            Assert.IsNotNull(def2);
            Assert.IsTrue(def1.id == def2.id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetWorkflowDefinitionByKey()
        {
            // Arrange

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());
            string definitionId = GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessDefinitionResponseData definition = this._ProcessEngine.GetWorkflowDefinitionByKey(DEFINITIONKEY_CREATETIMERSPROCESS, true).data.FirstOrDefault();

            // Assert
            Assert.IsNotNull(definition);
            Assert.IsNotNull(definition.key == DEFINITIONKEY_CREATETIMERSPROCESS);
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
            vars.Add("duration", "30000");

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
            vars.Add("throwException", "false");

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
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "30000");

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);
            var instanceNew = this._ProcessEngine.InvokeWorkflowInstance(definitionid, vars);

            var instances = _ProcessEngine.GetWorkflowInstances();
            var id = instances.data[0].id;
            var instance = _ProcessEngine.GetWorkflowInstance(id);

            bool cancelled = _ProcessEngine.DeleteWorkflowInstance(instance.id);


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
            vars.Add("duration", "10000");

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);
            System.Threading.Thread.Sleep(WAIT_TIMEOUT_MILLISECONDS);
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
            vars.Add("duration", "10000");

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);
            System.Threading.Thread.Sleep(WAIT_TIMEOUT_MILLISECONDS);
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
            vars.Add("duration", "10");
            //string expectedReturnVariable = "returnVariableName from definitionid-workflow"; // TODO: Use a workflow which returns a result
            //string expectedReturnValue = "expected value"; // TODO: Expected value

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_EXCEPTIONAFTERDURATIONSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);
            System.Threading.Thread.Sleep(WAIT_TIMEOUT_MILLISECONDS);
            ProcessInstanceResponseData instance = _ProcessEngine.GetWorkflowInstance(response.id);
            //ProcessVariableData variable = instance.variables.Where(v => v.name == expectedReturnVariable).FirstOrDefault();

            bool cancelled = _ProcessEngine.DeleteWorkflowInstance(response.id);

            // Assert


            // Assert
            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.completed);
            Assert.IsNotNull(instance.suspended);
            Assert.IsNotNull(instance.ended);
            Assert.IsFalse(instance.ended); // A failed workflow is not ended
            Assert.IsTrue(cancelled);
        }


        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetWorkflowResultFromSuspendedWorkflow()
        {
            // Arrange
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "30000");
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
            Assert.IsTrue(instance.suspended);

        }


        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetWorkflowResultFromRunningWorkflowFail()
        {
            // Arrange
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "30000");
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

        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetWorkflowResultWithFalseWorkflowId()
        {
            // Arrange
            string unknownId = "0";

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());
            ProcessInstanceResponseData instance = null;
            try
            {
                instance = _ProcessEngine.GetWorkflowInstance(unknownId);
                Assert.Fail("We should never reach this line...");
            }
            catch (Exception ex)
            {
                Assert.IsNull(instance);
                Assert.IsTrue(ex.Message == "Response status code does not indicate success: 404 (Not Found).");
            }


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
            vars.Add("duration", "60000");

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
            vars.Add("duration", "10000");

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
            vars.Add("duration", "10000");

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);
            System.Threading.Thread.Sleep(WAIT_TIMEOUT_MILLISECONDS); // Wait, till precess finished...

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
            vars.Add("duration", "10");

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_EXCEPTIONAFTERDURATIONSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);

            System.Threading.Thread.Sleep(WAIT_TIMEOUT_MILLISECONDS); // Wait, till precess finished...

            ProcessInstanceResponseData instance = _ProcessEngine.GetWorkflowInstance(response.id);
            bool failedBeforeCanceling = !instance.completed && !instance.ended;

            Assert.IsTrue(failedBeforeCanceling);

            bool cancelled = _ProcessEngine.DeleteWorkflowInstance(response.id);

            // Assert
            Assert.IsTrue(cancelled);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CancelEndedWorkflowFail()
        {
            // Arrange
            var definitionid = "will-be-determined-at-runtime";
            var vars = new Hashtable();
            vars.Add("duration", "10000");

            // Act
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn());

            definitionid = this.GetDefinitionId(DEFINITIONKEY_CREATETIMERSPROCESS);

            ProcessInstanceResponseData response = _ProcessEngine.InvokeWorkflowInstance(definitionid, vars);
            System.Threading.Thread.Sleep(WAIT_TIMEOUT_MILLISECONDS);

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
            // There has to be at least one test without annotation '[TestCategory("SkipOnTeamCity")]' per project
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Returns the definitionid of a given definitionkey.
        /// If definitionkey is DEFINITIONKEY_CREATETIMERSPROCESS and definition does not exist, the definition is deployed automatically.
        /// </summary>
        /// <param name="definitionkey">Something like createTimersProcess (see const  DEFINITIONKEY_CREATETIMERSPROCESS)</param>
        /// <returns> Something like "createTimersProcess:1:31" (version can change)</returns>
        private string GetDefinitionId(string definitionkey)
        {
            ProcessDefinitionResponseData definition = this._ProcessEngine.GetWorkflowDefinitionByKey(definitionkey, true).data.FirstOrDefault();

            if (definition == null && definitionkey == DEFINITIONKEY_CREATETIMERSPROCESS)
            {
                // Deploy the unexisting process definition to make tests.
                string filename = @"Resources\createTimersProcessUnitTests.bpmn20.xml";

                byte[] bytes = File.ReadAllBytes(filename);
                DeploymentResponseData response = this._ProcessEngine.CreateDeployment(filename, bytes);

                definition = this._ProcessEngine.GetWorkflowDefinitionByKey(definitionkey, true).data.FirstOrDefault();
            }

            if (definition == null && definitionkey == DEFINITIONKEY_CREATETIMERSPROCESS)
            {
                // Deploy the unexisting process definition to make tests.
                string filename = @"Resources\createTimersProcessUnitTests4.bpmn20.xml";

                byte[] bytes = File.ReadAllBytes(filename);
                DeploymentResponseData response = this._ProcessEngine.CreateDeployment(filename, bytes);

                definition = this._ProcessEngine.GetWorkflowDefinitionByKey(definitionkey, true).data.FirstOrDefault();
            }

            if (definition == null && definitionkey == DEFINITIONKEY_EXCEPTIONAFTERDURATIONSPROCESS)
            {
                // Deploy the unexisting process definition to make tests.
                string filename = @"Resources\exceptionAfterDurationProcessUnitTests.bpmn20.xml";

                byte[] bytes = File.ReadAllBytes(filename);
                DeploymentResponseData response = this._ProcessEngine.CreateDeployment(filename, bytes);

                definition = this._ProcessEngine.GetWorkflowDefinitionByKey(definitionkey, true).data.FirstOrDefault();
            }

            return definition.id;
        }

        #endregion
    }
}
