using System;
using System.Configuration;
using System.Threading;
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

        #region TokenTimeoutTest
        [Test]
        public void TokenTimeoutTest()
        {
            var client = EwsRestGateway.Connect(RestEndpointAddress, RestUserName, RestPassword);
            Assert.IsTrue(client.HasValidCredentials);
            WaitFor(null, 65000, false);

            Assert.IsTrue(client.HasTokenExpired());
            client.ReAuthenticate();

            var task = client.Root.RetrieveWithHttpMessagesAsync();
            try
            {
                task.Wait();

                var result = task.Result;
                Assert.Fail("This should have caused an unathorized exception");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            // Reauthenticate
            client.ReAuthenticate();
            Assert.IsTrue(client.HasValidCredentials);
        }
        #endregion

        #region WaitFor
        /// <summary>
        /// Waits for exitCondition to return a true value.  Timeout is 10 seconds by default
        /// </summary>
        public static void WaitFor(Func<bool> exitCondition, int timeout = 10000, bool assertOnTimeout = true)
        {
            var startedAt = DateTimeOffset.UtcNow;
            do
            {
                if (exitCondition != null && exitCondition()) return;

                NoBusyWait(50);
            } while ((DateTimeOffset.UtcNow - startedAt).TotalMilliseconds < timeout);

            if (assertOnTimeout) Assert.Fail("Timed out waiting for action to occur");
        }
        #endregion
        #region NoBusyWait (ISmartConnectorTestFixture)
        /// <summary>
        /// Waits until duration has elapsed but allows procesing to continue by 
        /// </summary>
        /// <param name="duration">Duration in mSec to wait</param>
        public static void NoBusyWait(long duration)
        {
            var startedAt = DateTime.Now;
            do
            {
                Thread.Sleep(50);
            } while ((DateTime.Now - startedAt).TotalMilliseconds < duration);
        }
        #endregion
    }
}
