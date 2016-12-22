using Ews.Server.Contract.Processor;
using Mongoose.Ews.Server;

namespace SmartConnector.WeatherExtension.EwsServer
{
    public class CustomDataExchange : MongooseDataExchange
    {
        #region CreateSetValuesProcessor - Override
        /// <summary>
        /// Override a base Create Processor method to inject your custom request processor logic.
        /// </summary>
        protected override SetValuesProcessor CreateSetValuesProcessor()
        {
            return new CustomSetValuesProcessor();
        }
        #endregion
    }
}
