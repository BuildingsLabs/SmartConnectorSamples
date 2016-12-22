using System.Collections.Generic;
using System.Linq;
using Ews.Client;
using Ews.Common;
using Mongoose.Ews.Server.Data;
using NUnit.Framework;
using Mongoose.Process.Test;
using SmartConnector.WeatherExtension.EwsServer;

namespace SmartConnector.WeatherExtension.Test
{
    [TestFixture]
    public class CustomEwsServeTestFixture : ISmartConnectorTestFixture
    {
        private const string UserName = "admin";
        private const string Password = "AdMin1234";
        private const string EwsEndpoint = "http://localhost:50999/MyRoute";

        #region FixtureSetup
        [OneTimeSetUp]
        public void FixtureSetup()
        {
            this.ConfigureTestFixture();
        }
        #endregion

        #region CreateNewEwsServerTest
        [Test]
        public void CreateNewEwsServerTest()
        {
            const string serverName = "Custom EWS Test Server";
            try
            {
                var server = BootstrapEwsServer(serverName);

                // Normally, the output of this assembly would be deployed to the SmartConnector service folder and SmartConnector would manage hosting the EWS Server endpoint.  
                // Since we aren't doing that at the moment, we need to intervene on it's behalf so we can debug our custom controller.
                using (var host = new CustomEwsServiceHost(server))
                {
                    host.Open();

                    // But why is it still not "running"?
                    Assert.IsFalse(server.IsRunning);

                    // Because we're looking at a "stale" copy of the server.  Adapters shouldn't be kept open for long periods of time or may be looking at stale data.
                    // Let's reconnect
                    using (var adapter = EwsServerDataAdapter.ConnectExisting(serverName, UserName, Password))
                    {
                        server = adapter.Server;
                        // But it's still not running?  Unfortunately, the interaction to give this feedback IS managed by SmartConnector so even though we spun up an endpoint it's not reporting that.  Let's verify that.
                        Assert.IsFalse(server.IsRunning);
                        server = null;
                    }

                    // We can use EwsClient to communicate with our server though
                    TestServerWithEwsClient();

                    host.Close();
                }
            }
            finally
            {
                using (var adapter = EwsServerDataAdapter.ConnectExisting(serverName, UserName, Password))
                {
                    // Let's clean up after ourselves.
                    adapter.DeleteServer();
                }
            }
        }

        #endregion

        #region BootstrapEwsServer
        private Mongoose.Ews.Server.Data.EwsServer BootstrapEwsServer(string serverName)
        {
            var type = typeof(CustomEwsServiceHost);
            var className = type.FullName;
            var assembleyFile = $"{type.Assembly.GetName().Name}.dll";

            using (var adapter = EwsServerDataAdapter.ConnectNew(serverName, EwsEndpoint, "MyRealm", UserName, Password, true, true, assembleyFile, className))
            {
                adapter.PurgeData();

                // If you use the Adapter to access your service, you will get standard behavior.
                var vi = adapter.ValueItems.FirstOrDefault(x => x.AlternateId == ServerHelper.CityId);
                Assert.IsNull(vi);

                // Let's add the ValueItem our custom SetValuesProcessor is targeting
                var addedVi = adapter.AddValueItem(ServerHelper.CityId, "City", null, EwsValueTypeEnum.String, EwsValueWriteableEnum.Writeable, EwsValueForceableEnum.NotForceable, EwsValueStateEnum.Good, null);
                Assert.That(addedVi.Value, Is.Null.Or.Empty);

                // Add CityNameId
                addedVi = adapter.AddValueItem(ServerHelper.CityNameId, "City Name", null, EwsValueTypeEnum.String, EwsValueWriteableEnum.ReadOnly, EwsValueForceableEnum.NotForceable, EwsValueStateEnum.Good, null);
                Assert.That(addedVi.Value, Is.Null.Or.Empty);
                Assert.AreEqual(EwsValueStateEnum.Good, addedVi.State);

                // Even though we told the Server to auto start, it isn't running.  Why?  Because we're in a test harness even if SmartConnector is running in the background.
                Assert.IsFalse(adapter.Server.IsRunning);

                return adapter.Server;
            }
        }
        #endregion
        #region TestServerWithEwsClient
        private void TestServerWithEwsClient()
        {

            using (var c = CreateConnection())
            {
                var readResults = c.GetValues(new[] { ServerHelper.CityId, ServerHelper.CityNameId });
                Assert.IsNotNull(readResults);
                Assert.IsNotNull(readResults.GetValuesItems);
                Assert.IsNotNull(readResults.GetValuesItems);
                Assert.AreEqual(2, readResults.GetValuesItems.Length);

                var cityVI = readResults.GetValuesItems.FirstOrDefault(x => x.Id == ServerHelper.CityId);
                Assert.IsNotNull(cityVI);

                var cityNameVI = readResults.GetValuesItems.FirstOrDefault(x => x.Id == ServerHelper.CityNameId);
                Assert.IsNotNull(cityNameVI);

                // Call our write method against the CityId point giving it a new value.
                var writePoints = new List<ValueTypeStateless>
                        {
                            new ValueTypeStateless
                            {
                                Id= ServerHelper.CityId,
                                Value = "-1"
                            }
                        };
                var writeResults = c.SetValues(writePoints.ToArray());
                Assert.IsNotNull(writeResults);
                Assert.IsNotNull(writeResults.SetValuesResults);
                Assert.AreEqual(1, writeResults.SetValuesResults.Length);
                Assert.AreEqual(ServerHelper.CityId, writeResults.SetValuesResults[0].Id);
                Assert.IsTrue(writeResults.SetValuesResults[0].Success);

                // Re-read CityNameId. 
                readResults = c.GetValues(new[] { ServerHelper.CityNameId });
                Assert.IsNotNull(readResults);
                Assert.IsNotNull(readResults.GetValuesItems);
                Assert.AreEqual(readResults.GetValuesItems.Length, 1);
                var soapVi = readResults.GetValuesItems[0];
                Assert.AreEqual(ServerHelper.CityNameId, soapVi.Id);
                Assert.AreEqual("Pending", soapVi.Value);

                // EWS returns strings here.  Fortunately, there's extension methods to help you out.
                Assert.AreEqual(EwsValueStateEnum.Uncertain.ToEwsString(), soapVi.State);
            }
        } 
        #endregion

        #region CreateConnection
        private EwsClient CreateConnection(string username = UserName, string password = Password)
        {
            var creds = new EwsSecurity
            {
                UserName = username,
                Password = password,
            };
            return new EwsClient(creds, EwsEndpoint);
        }
        #endregion
    }
}
