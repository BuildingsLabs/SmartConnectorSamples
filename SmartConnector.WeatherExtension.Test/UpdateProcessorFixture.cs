using Mongoose.Process.Test;
using NUnit.Framework;

namespace SmartConnector.WeatherExtension.Test
{
    [TestFixture]
    public class UpdateProcessorFixture : IProcessorTestFixture<UpdateProcessor>
    {
        #region FixtureSetup
        [OneTimeSetUp]
        public void FixtureSetup()
        {
            this.ConfigureTestFixture();
        }
        #endregion
        #region CreateTestableProcessor (IProcessorFixture Member)
        public UpdateProcessor CreateTestableProcessor()
        {
            var processor = this.CreateProccessorInstanceWithDefaultValues();

            processor.ApiKey = "d1ce3419db160685c4a88244e1b1c24e"; // TODO - Put your API Key here
            processor.UpdateForecast = true;
            processor.UpdateCurrentConditions = true;
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
            var tempValue = processor.ApiKey;
            processor.ApiKey = null;
            Assert.AreEqual(1, this.ValidateProcessor(processor).Count);
            processor.ApiKey = tempValue;

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
