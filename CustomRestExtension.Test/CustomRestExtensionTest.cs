using System.Linq;
using Mongoose.Configuration;
using Mongoose.Test.Processors;
using Mongoose.Test.Rest;
using NUnit.Framework;
using SmartConnectorService = Mongoose.Service.Mongoose;

namespace CustomRestExtension.Test
{
    public class CustomRestExtensionTest : RestExtensionTestFixtureBase<MyRestProvider>
    {
        #region FixtureOneTimeSetup_Base - Override
        protected override void FixtureOneTimeSetup_Base()
        {
            SmartConnectorService.InitIoC();
        }
        #endregion

        #region CreateRunnableConfiguration - Override
        protected override RestConfiguration CreateRunnableConfiguration()
        {
            var config  = base.CreateRunnableConfiguration();

            config.AssertParameterExistsAndSetValue("Scheme", "http");
            config.AssertParameterExistsAndSetValue("Host", "localhost");
            config.AssertParameterExistsAndSetValue("Port", "8086");

            Assert.IsNotNull(config.ParameterSets);
            Assert.AreEqual(1, config.ParameterSets.Count);
            var set = config.ParameterSets.FirstOrDefault(x => x.Name == "HttpConfiguration");
            Assert.IsNotNull(set);
            set.AssertParameterExistsAndSetValue("Name", "Custom REST Extension Endpoint");
            set.AssertParameterExistsAndSetValue("AccessTokenExpireTimeSpanMinutes", "60");
            set.AssertParameterExistsAndSetValue("ServeSwaggerMetadata", "True");

            set = set.ParameterSets.FirstOrDefault(x => x.Name == "ClientCredentials");
            Assert.IsNotNull(set);
            set.AssertParameterExistsAndSetValue("UserName", "admin");
            set.AssertParameterExistsAndSetValue("Password", "Admin!23");

            return config;
        }
        #endregion

        #region StandupEndpointTest - Override
        [Test]
        public override void StandupEndpointTest()
        {
            base.StandupEndpointTest();
        } 
        #endregion
    }
}
