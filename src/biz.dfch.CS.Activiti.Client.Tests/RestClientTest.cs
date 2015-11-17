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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Activiti.Client.Tests
{
    [TestClass]
    public class RestClientTest
    {
        [TestMethod]
        public void CreatingClientWithParameterlessConstructorSucceeds()
        {
            // Arrange

            // Act
            var restClient = new RestClient();
            
            // Assert
            Assert.IsNotNull(restClient);
        }

        [TestMethod]
        public void CreatingClientWithParamterisedConstructorSucceeds()
        {
            // Arrange
            var serveruri = new Uri("http://192.168.112.129:9000/activiti-rest/service/");
            var username = "kermit";
            var password = "kermit";
            
            // Act
            var restClient = new RestClient(serveruri, username, password);
            restClient.Login();
            
            // Assert
            Assert.IsNotNull(restClient);
            Assert.AreEqual(serveruri, restClient.UriServer);
            Assert.AreEqual(username, restClient.Username);
            //Assert.AreEqual(password, restClient.Password);
        }

        [TestMethod]
        public void CreatingClientAndLoginSucceeds()
        {
            // Arrange
            var serveruri = new Uri("http://192.168.112.129:9000/activiti-rest/service/");
            var username = "kermit";
            var password = "kermit";

            // Act
            var restClient = new RestClient(serveruri, username, password);
            var user = restClient.Login();

            // Assert
            Assert.IsNotNull(restClient);
            Assert.AreEqual(serveruri, restClient.UriServer);
            Assert.AreEqual(username, restClient.Username);
            //Assert.AreEqual(password, restClient.Password);
        }
    }
}
