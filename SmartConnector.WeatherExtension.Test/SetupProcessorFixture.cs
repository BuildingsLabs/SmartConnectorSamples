using Mongoose.Common;
using Mongoose.Process.Test;
using NUnit.Framework;

namespace SmartConnector.WeatherExtension.Test
{
    [TestFixture]
    public class SetupProcessorFixture : IProcessorTestFixture<SetupProcessor>
    {
        #region FixtureSetup
        [OneTimeSetUp]
        public void FixtureSetup()
        {
            this.ConfigureTestFixture();
        }
        #endregion

        #region CreateTestableProcessor (IProcessorFixture Member)
        /// <summary>
        /// Returns an instance of SetupProcessor which was configured using the default values for all properties.
        /// </summary>
        /// <returns></returns>
        public SetupProcessor CreateTestableProcessor()
        {
            var processor = this.CreateProccessorInstanceWithDefaultValues();

            // We'll supply the address of the EWS Server we will provision because no default is available.
            processor.EwsAddress = "http://localhost:56731/SmartConnectorWeather";

            return processor;
        }
        #endregion

        #region ValidateTest (IProcessorFixture Member)
        [Test]
        public void ValidateTest()
        {
            var processor = CreateTestableProcessor();

            // Verify that CreateTestableProcessor has no issues
            var issues = this.ValidateProcessor(processor);
            Assert.AreEqual(0, issues.Count);

            // Now, start messing with the processor to verify it Validates things we want validated.
            var tempValue = processor.EwsAddress;
            processor.EwsAddress = null;
            Assert.AreEqual(1, this.ValidateProcessor(processor).Count);
            processor.EwsAddress = tempValue;

            // Add more validation as needed
        }
        #endregion

        #region ExecuteTest (IProcessorFixture Member)
        [Test]
        public void ExecuteTest()
        {
            this.RunExecuteTest();
        }
        #endregion
        #region CancelTest (IProcessorFixture Member)
        [Test]
        public void CancelTest()
        {
            this.RunCancelTest();
        }
        #endregion
    }
}
