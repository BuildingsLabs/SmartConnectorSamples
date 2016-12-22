using Ews.Server.Contract;
using Mongoose.Common;
using Mongoose.Ews.Server.Processor;

namespace SmartConnector.WeatherExtension.EwsServer
{
    public class CustomSetValuesProcessor : MongooseSetValuesProcessor
    {
        #region SetValues - Override
        protected override ResultType SetValue(ValueTypeStateless item)
        {
            if (item.Id == ServerHelper.CityId)
            {
                // Set all appropriate data to an Uncertain state until the SetProcessor updates the data from the weather service.
                ServerHelper.MakeDataServedUncertain(DataAdapter, "Pending", LogCategory.EwsServe);
            }
            return base.SetValue(item);
        }
        #endregion
    }
}