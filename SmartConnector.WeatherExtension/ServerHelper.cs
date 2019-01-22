using System;
using System.Collections.Generic;
using Ews.Common;
using Mongoose.Ews.Server.Data;
using SxL.Common;

namespace SmartConnector.WeatherExtension
{
    public static class ServerHelper
    {
        public const string CityId = "CityCode";
        public const string CityNameId = "CityName";
        public const string CurrentLastUpdateId = "CurrentConditions.LastUpdated";
        public const string CurrentTemperatureId = "CurrentConditions.Temperature";
        public const string CurrentPressureId = "CurrentConditions.Pressure";
        public const string CurrentRelativeHumidityId = "CurrentConditions.RelativeHumidity";
        public const string CurrentTemperatureHistoryId = "CurrentConditions.Temperature.History";
        public const string CurrentPressureHistoryId = "CurrentConditions.Pressure.History";
        public const string CurrentRelativeHumidityHistoryId = "CurrentConditions.RelativeHumidity.History";
        public const int ForecastNumberOfDays = 3;
        public const string ForecastDayContainerId = "Forecast.Day{0}";
        public const string ForecastDayDateId = "Forecast.Day{0}.Date";
        public const string ForecastDayHighTempId = "Forecast.Day{0}.HighTemp";
        public const string ForecastDayLowTempId = "Forecast.Day{0}.LowTemp";
        public const string ForecastDayPressureId = "Forecast.Day{0}.Pressure";
        public const string ForecastDayRelativeHumidityId = "Forecast.Day{0}.RelativeHumidity";
        public const string ForecastDayDescriptionId = "Forecast.Day{0}.Description";

        #region VolatileValueItemIds
        private static List<string> _volatileValueItemIds;
        /// <summary>
        /// Returns a list of ValueItem ID values which are "volatile".  Those that are affected by a change in CityId or a lack of response from the service.
        /// </summary>
        public static List<string> VolatileValueItemIds
        {
            get
            {
                if (_volatileValueItemIds != null) return _volatileValueItemIds;
                _volatileValueItemIds = new List<string>
                {
                    CurrentLastUpdateId,
                    CurrentTemperatureId,
                    CurrentPressureId,
                    CurrentRelativeHumidityId,
                    CurrentTemperatureHistoryId,
                    CurrentPressureHistoryId
                };
                for (var day = 1; day <= ForecastNumberOfDays; day++)
                {
                    _volatileValueItemIds.Add(string.Format(ForecastDayContainerId, day));
                    _volatileValueItemIds.Add(string.Format(ForecastDayDateId, day));
                    _volatileValueItemIds.Add(string.Format(ForecastDayHighTempId, day));
                    _volatileValueItemIds.Add(string.Format(ForecastDayLowTempId, day));
                    _volatileValueItemIds.Add(string.Format(ForecastDayPressureId, day));
                    _volatileValueItemIds.Add(string.Format(ForecastDayRelativeHumidityId, day));
                    _volatileValueItemIds.Add(string.Format(ForecastDayDescriptionId, day));
                }
                return _volatileValueItemIds;
            }
        }
        #endregion

        #region MakeDataServedUncertain
        /// <summary>
        /// When the city is changed and/or we don't get a reply from the service, we don't know if the current ValueItem data is correct or not.  
        /// Let's set the State to Uncertain.
        /// </summary>
        public static void MakeDataServedUncertain(EwsServerDataAdapter adapter, string cityName, string logCategory)
        {
            adapter.ModifyValueItemValue(CityNameId, cityName, EwsValueStateEnum.Uncertain);

            foreach (var id in VolatileValueItemIds)
            {
                // In case the ID doesn't resolve to a VI (as in UT environments), let's just log that condition.  
                try
                {
                    adapter.ModifyValueItemState(id, EwsValueStateEnum.Uncertain);
                }
                catch (Exception ex)
                {
                    Logger.LogError(logCategory, ex);
                }
            }
        }
        #endregion
    }
}
