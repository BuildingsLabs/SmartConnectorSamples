using System;
using Mongoose.Common;
using Mongoose.Configuration;
using Mongoose.Test;
using Mongoose.Test.Processors;
using NUnit.Framework;
using SxL.Common;
using SmartConnectorService = Mongoose.Service.Mongoose;

namespace SmartConnector.UtilityExtensions.Test
{
    [TestFixture]
    public class EwsDiscoveryProcessorFixture : SmartConnectorTestFixtureBase, IProcessorTestFixture<EwsDiscoveryProcessor>
    {
        #region FixtureOneTimeSetup_Base - Override
        protected override void FixtureOneTimeSetup_Base()
        {
            base.FixtureOneTimeSetup_Base();
            SmartConnectorService.InitIoC();
        } 
        #endregion

        #region CreateTestableProcessor (IProcessorTestFixture Member)
        private EwsDiscoveryProcessor _processor;
        public EwsDiscoveryProcessor CreateTestableProcessor()
        {
            if (_processor != null) return _processor;
            Type processorType;
            ActivatorHelper.ActivateObject<EwsDiscoveryProcessor>("SmartConnector.UtilityExtensions.dll", "SmartConnector.UtilityExtensions.EwsDiscoveryProcessor", out processorType);
            var config = ProcessConfiguration.ExtractConfiguration(processorType);
            _processor = config.InstantiateInstance<EwsDiscoveryProcessor>();
            _processor.OutputFilePath = @"%PROGRAMDATA%\SmartConnector\SBO Output.txt";
            _processor.Address = @"http://localhost:8081/EcoStruxure/DataExchange";
            _processor.UserName = "admin";
            _processor.Password = "Admin!23";
            return _processor;
        }
        #endregion

        #region ValidateTest (IProcessorTestFixture Member)
        [Test]
        public void ValidateTest()
        {
            Logger.LogDebug(LogCategory.Testing, "EwsDiscoveryProcessorFixture.ValidateTest Start");
            var processor = CreateTestableProcessor();
            var results = GenericValidator.ValidateItem(processor);
            Assert.AreEqual(0, results.Count);
            Logger.LogDebug(LogCategory.Testing, "EwsDiscoveryProcessorFixture.ValidateTest Completed");
        }
        #endregion

        #region CancelTest (IProcessorTestFixture Member)
        [Test]
        public void CancelTest()
        {
            Logger.LogDebug(LogCategory.Testing, "EwsDiscoveryProcessorFixture.CancelTest Start");
            Assert.Pass();
            Logger.LogDebug(LogCategory.Testing, "EwsDiscoveryProcessorFixture.CancelTest Completed");
        }
        #endregion

        #region ExecuteTest (IProcessorTestFixture Member)
        [Test]
        public void ExecuteTest()
        {
            const int fiveMinutes = 60 * 5 * 1000;

            Logger.LogDebug(LogCategory.Testing, "EwsDiscoveryProcessorFixture.ExecuteTest Start");
            this.RunExecuteTest(cancelTimeout: fiveMinutes);
            Logger.LogDebug(LogCategory.Testing, "EwsDiscoveryProcessorFixture.ExecuteTest Completed");
        }
        #endregion
    }
}
