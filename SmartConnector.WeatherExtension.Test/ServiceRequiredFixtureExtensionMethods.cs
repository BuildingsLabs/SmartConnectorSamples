using Mongoose.Common;
using Mongoose.Process.Test;
using SxL.Common;
using System;
using SmartConnectorRuntime = Mongoose.Service.Mongoose;

namespace SmartConnector.WeatherExtension.Test
{
    public static class ServiceRequiredFixtureExtensionMethods
    {
        #region ConfigureTestFixture (ISmartConnectorTestFixture)
        /// <summary>
        /// Common code for any ISmartConnectorTestFixture requiring IoC availability from a referenced SmartConnector.
        /// </summary>
        public static void ConfigureTestFixture(this ISmartConnectorTestFixture fixture)
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
