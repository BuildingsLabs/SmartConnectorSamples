using System;
using System.Configuration;
using Ews.RestExtensions.Client.Proxy;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Ews.RestExtensions.Client.Test
{
    [TestFixture]
    public class ProxyTestFixture
    {
        public const string RestEndpointAddress = "http://127.0.0.1:8083";
        public const string RestUserName = "admin";
        public const string RestPassword = "Admin!23";

        #region BadCredentialsTest
        [Test]
        public void BadCredentialsTest()
        {
            var client = EwsRestGateway.Connect(RestEndpointAddress, RestUserName, "Bad Password");
            Assert.IsFalse(client.HasValidCredentials);
            Assert.IsNull(client.Credentials);
        }
        #endregion

        #region GetRootTest
        [Test]
        public void GetRootTest()
        {
            var client = EwsRestGateway.Connect(RestEndpointAddress, RestUserName, RestPassword);
            Assert.IsTrue(client.HasValidCredentials);

            var task = client.Root.RetrieveWithHttpMessagesAsync();
            task.Wait();
            var result = task.Result;
            Assert.IsNotNull(result);
            var root = result.Body;
            Assert.IsNotNull(root);
            Assert.AreEqual("ContainerItem0", root.Id);
            Assert.AreEqual("SmartConnector EWS Test Server", root.Name);
            Assert.AreEqual("All folders derive from here", root.Description);
        }
        #endregion
    }
}
