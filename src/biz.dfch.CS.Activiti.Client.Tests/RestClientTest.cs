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
            var server = new Uri("http://localhost");
            var username = "arbitrary-user";
            var password = "Ym9ndXMtcGFzc3dvcmQ=";
            
            // Act
            var restClient = new RestClient(server, username, password);

            // Assert
            Assert.IsNotNull(restClient);
            Assert.AreEqual(server, restClient.Server);
            Assert.AreEqual(username, restClient.Username);
            //Assert.AreEqual(password, restClient.Password);
        }
    }
}
