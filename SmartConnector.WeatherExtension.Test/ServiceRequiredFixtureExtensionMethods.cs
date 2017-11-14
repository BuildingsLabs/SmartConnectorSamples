using Mongoose.Common;
using SxL.Common;
using System;
using Mongoose.Test;
using SmartConnectorRuntime = Mongoose.Service.Mongoose;

namespace SmartConnector.WeatherExtension.Test
{
    public static class ServiceRequiredFixtureExtensionMethods
    {
        #region ConfigureTestFixture (SmartConnectorTestFixtureBase)
        /// <summary>
        /// Common code for any SmartConnectorTestFixtureBase requiring IoC availability from a referenced SmartConnector.
        /// </summary>
        public static void ConfigureTestFixture(this SmartConnectorTestFixtureBase fixture)
        {
            try
            {
                SmartConnectorRuntime.InitDataDirectory();
                SmartConnectorRuntime.InitIoC();
            }
            catch (Exception ex)
            {
                Logger.LogError(LogCategory.Testing, ex);
                throw;
            }
        }
        #endregion
    }
}
