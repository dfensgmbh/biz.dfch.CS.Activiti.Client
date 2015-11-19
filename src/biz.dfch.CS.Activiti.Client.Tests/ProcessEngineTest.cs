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

namespace biz.dfch.CS.Activiti.Client.Tests
{
    [TestClass]
    public class ProcessEngineTest
    {

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

        [TestMethod]
        public void LoginWithWrongUsernameAndPassword()
        {
            this._ProcessEngine.Login("wrongusername", "1234");
            Assert.IsTrue(!this._ProcessEngine.IsLoggedIn);
        }


        [TestMethod]
        public void Login()
        {
            this._ProcessEngine.Login(username, password);
            Assert.IsTrue(this._ProcessEngine.IsLoggedIn);
        }



        //[TestMethod]
        //public void CreatingProcessEngineWithParameterlessConstructorSucceeds()
        //{
        //    // Arrange

        //    // Act
        //    var processEngine = new ProcessEngine();

        //    // Assert
        //    Assert.IsNotNull(processEngine);

        //}

        //[TestMethod]
        //public void CreatingProcessEngineWithParamterisedSucceeds()
        //{
        //    // Arrange
        //    var serveruri = new Uri("http://192.168.112.129:9000/activiti-rest/service/");
        //    var username = "kermit";
        //    var password = "kermit";

        //    // Act
        //    var processEngine = new ProcessEngine(serveruri, username, password);

        //    // Assert
        //    Assert.IsNotNull(processEngine);
        //    Assert.AreEqual(serveruri, processEngine.Client.UriServer);
        //    Assert.AreEqual(username, processEngine.Client.Username);
        //    //Assert.AreEqual(password, restClient.Password);
        //}

        //[TestMethod]
        //public void CreatingProcessEngineWithClientSucceeds()
        //{
        //    // Arrange
        //    var serveruri = new Uri("http://192.168.112.129:9000/activiti-rest/service/");
        //    var username = "kermit";
        //    var password = "kermit";

        //    // Act
        //    var restClient = new RestClient(serveruri, username, password);
        //    var processEngine = new ProcessEngine(restClient);
        //    processEngine.Login();

        //    // Assert
        //    Assert.IsNotNull(restClient);
        //    Assert.IsNotNull(processEngine);
        //    Assert.AreEqual(serveruri, restClient.UriServer);
        //    Assert.AreEqual(username, restClient.Username);
        //    Assert.AreEqual(username, processEngine.Client.Username);
        //    //Assert.AreEqual(password, restClient.Password);
        //}

        //[TestMethod]
        //public void CreatingProcessEngineAndLoginSucceeds()
        //{
        //    // Arrange
        //    var serveruri = new Uri("http://192.168.112.129:9000/activiti-rest/service/");
        //    var username = "kermit";
        //    var password = "kermit";

        //    // Act
        //    var processEngine = new ProcessEngine(serveruri, username, password);
        //    processEngine.Login(username, password);

        //    // Assert
        //    Assert.IsNotNull(processEngine);
        //    Assert.AreEqual(serveruri, processEngine.Client.UriServer);
        //    Assert.AreEqual(username, processEngine.Client.Username);
        //    //Assert.AreEqual(password, restClient.Password);
        //}

        [TestMethod]
        public void GetWorkflowDefinitions()
        {
            // Arrange
           

            // Act
           this._ProcessEngine.Login(username, password);
           Assert.IsTrue(this._ProcessEngine.IsLoggedIn);
           var wdefObj1 = this._ProcessEngine.GetWorkflowDefinitions<ProcessDefinitionsResponse>();
           var wdefObj2 = this._ProcessEngine.GetWorkflowDefinitions();

            // Assert
            Assert.IsNotNull(wdefObj1);
            Assert.IsNotNull(wdefObj2);
        }

    }
}
