using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ews.Common;
using Mongoose.Common;
using Mongoose.Common.Attributes;
using Mongoose.Ews.Server.Data;
using Mongoose.Ews.Server.Data.Shared;

namespace SmartConnector.WeatherExtension
{
    [ConfigurationDefaults("Weather Setup Processor", "Ensures the presence of an EWS Server and certain contents, bootstrapping whatever needs to be there.")]
    public class SetupProcessor : WeatherProcessorBase
    {
        #region EwsAddress
        /// <summary>
        /// The address 
        /// </summary>
        [Required, DefaultValue("http://localhost:5300/SmartConnectorWeatherService"), Tooltip("Default HTTP address to configure when bootstraping the EWS Server")]
        public string EwsAddress { get; set; }
        #endregion
        #region Realm
        [DefaultValue("CustomRealm"), Tooltip("Realm value for HTTP Digest Authentication")]
        public string Realm { get; set; }
        #endregion

        #region Execute_Subclass - Override
        protected override IEnumerable<Prompt> Execute_Subclass()
        {
            // Make sure we can connect to an EWS Server
            if (!IsConnected) return new List<Prompt> { CreateCannotConnectPrompt() };

            // If the server isn't running, start it now.
            if (!DataAdapter.Server.IsRunning) DataAdapter.StartServer();

            // Confirm that the server settings are how we want them be.
            EnsureServerParameters();

            // Add the fields a client can use to "set" the location
            AddUserInputFields();

            // Add the folders and data points where we'll write the current weather conditions (in UpdateProcessor)
            AddCurrentConditionPlaceHolders();

            // Add the folders and data points where we'll write the forecast (in UpdateProcessor)
            AddForecastPlaceHolders();

            // Return any issues
            return new List<Prompt>();
        }
        #endregion

        #region CreateEwsServer - Override
        protected override EwsServerDataAdapter CreateEwsServer()
        {
            return EwsServerDataAdapter.ConnectNew(ServerName, EwsAddress, Realm, UserName, Password, 
                true, true, "SmartConnector.WeatherExtension.dll", "SmartConnector.WeatherExtension.EwsServer.CustomEwsServiceHost");
        }
        #endregion

        #region EnsureServerParameters
        private void EnsureServerParameters()
        {
            CheckCancellationToken();

            DataAdapter.ModifyServerIsAutoStart(true);
            DataAdapter.ModifyServerAllowCookies(true);
            DataAdapter.ModifyServerPageSize(1000);
            DataAdapter.ModifyServerRootContainerItemAlternateId("RootContainer");
            DataAdapter.ModifyServerRootContainerItemDescription("All folders derive from here");
            EnsureSupportedMethods();
        }
        #endregion
        #region EnsureSupportedMethods
        /// <summary>
        /// Disable EWS Server functions that aren't needed in this solution and enable those that are.
        /// </summary>
        private void EnsureSupportedMethods()
        {
            CheckCancellationToken();

            DataAdapter.ModifyServerSupportedMethods(new EwsServerMethods
            {
                AcknowledgeAlarmEvents = false,
                ForceValues = false,
                GetAlarmEventTypes = false,
                GetAlarmEvents = false,
                GetAlarmHistory = false,
                GetEnums = false,
                GetHierarchicalInformation = false,
                GetUpdatedAlarmEvents = false,
                UnforceValues = false,

                GetContainerItems = true,
                GetHistory = true,
                GetItems = true,
                GetNotification = true,
                GetValues = true,
                GetWebServiceInformation = true,
                Renew = true,
                SetValues = true,
                Subscribe = true,
                Unsubscribe = true
            });
        }
        #endregion

        #region EnsureContainerItem
        private EwsContainerItem EnsureContainerItem(string altId, string name = null, string description = null, EwsContainerTypeEnum type = EwsContainerTypeEnum.Folder, EwsContainerItem parent = null)
        {
            CheckCancellationToken();

            var ci = DataAdapter.ContainerItems.FirstOrDefault(x => x.AlternateId == altId);
            if (ci == null) return DataAdapter.AddContainerItem(altId, name ?? altId, description, type, parent);
            ci = DataAdapter.ModifyContainerItemName(ci, altId);
            ci = DataAdapter.ModifyContainerItemDescription(ci, description);
            ci = DataAdapter.ModifyContainerItemType(ci, type);
            return DataAdapter.ModifyContainerItemParent(ci, parent);
        }
        #endregion
        #region EnsureValueItem
        private EwsValueItem EnsureValueItem(string altId, string name = null, string description = null, EwsValueTypeEnum type = EwsValueTypeEnum.String, EwsContainerItem parent = null, string unit = null, EwsValueWriteableEnum writeable = EwsValueWriteableEnum.Writeable, EwsValueForceableEnum forceable = EwsValueForceableEnum.Forceable, EwsValueStateEnum defaultState = EwsValueStateEnum.Uncertain)
        {
            CheckCancellationToken();

            var vi = DataAdapter.ValueItems.FirstOrDefault(x => x.AlternateId == altId);
            if (vi == null) return DataAdapter.AddValueItem(altId, name ?? altId, description, type, writeable, forceable, defaultState, unit, parent);
            vi = DataAdapter.ModifyValueItemName(vi, altId);
            vi = DataAdapter.ModifyValueItemDescription(vi, description);
            vi = DataAdapter.ModifyValueItemType(vi, type);
            vi = DataAdapter.ModifyValueItemWriteable(vi, writeable);
            vi = DataAdapter.ModifyValueItemForceable(vi, forceable);
            vi = DataAdapter.ModifyValueItemUnit(vi, unit);
            return DataAdapter.ModifyValueItemParent(vi, parent);
        }
        #endregion
        #region EnsureHistoryItem
        private EwsHistoryItem EnsureHistoryItem(string altId, string name, string description, EwsValueItem valueItem, EwsContainerItem parent = null)
        {
            CheckCancellationToken();

            var hi = DataAdapter.HistoryItems.FirstOrDefault(x => x.AlternateId == altId);
            if (hi == null) return DataAdapter.AddHistoryItem(altId, name ?? altId, description, valueItem, true, parent);
            hi = DataAdapter.ModifyHistoryItemName(hi, altId);
            hi = DataAdapter.ModifyHistoryItemDescription(hi, description);
            return DataAdapter.ModifyHistoryItemParent(hi, parent);
        }
        #endregion

        #region AddUserInputFields
        /// <summary>
        /// Add the ValueItems clients will be able to write to to change the forecast city
        /// </summary>
        private void AddUserInputFields()
        {
            var city = EnsureValueItem(ServerHelper.CityId, "City Code", null, EwsValueTypeEnum.Long, null, null, EwsValueWriteableEnum.Writeable, EwsValueForceableEnum.NotForceable);

            // The API we're using needs a location ID.  See http://bulk.openweathermap.org/sample/city.list.json.gz
            if (string.IsNullOrEmpty(city.Value) || city.GetValue() == 0) city = DataAdapter.ModifyValueItemValue(city, 4929055L);

            var cityName = EnsureValueItem(ServerHelper.CityNameId, "City Name", null, EwsValueTypeEnum.String, null, null,
                EwsValueWriteableEnum.ReadOnly, EwsValueForceableEnum.NotForceable);
            if (string.IsNullOrEmpty(cityName.Value)) cityName = DataAdapter.ModifyValueItemValue(cityName, string.Empty, EwsValueStateEnum.Uncertain);

        }
        #endregion
        #region AddCurrentConditionPlaceHolders
        private void AddCurrentConditionPlaceHolders()
        {
            var currentFolder = EnsureContainerItem("CurrentConditions");
            EnsureValueItem(ServerHelper.CurrentLastUpdateId, "Last Updated", null, EwsValueTypeEnum.DateTime, currentFolder, null,
                EwsValueWriteableEnum.ReadOnly, EwsValueForceableEnum.NotForceable);

            // Temperature
            var currentTemp = EnsureValueItem(ServerHelper.CurrentTemperatureId, "Temperature", null, EwsValueTypeEnum.Double, currentFolder, "°C",
                EwsValueWriteableEnum.ReadOnly, EwsValueForceableEnum.NotForceable);
            EnsureHistoryItem(ServerHelper.CurrentTemperatureHistoryId, null, null, currentTemp, currentFolder);

            // Pressure
            var currentPressure = EnsureValueItem(ServerHelper.CurrentPressureId, "Pressure", null, EwsValueTypeEnum.Double, currentFolder, "hPA",
                EwsValueWriteableEnum.ReadOnly, EwsValueForceableEnum.NotForceable);
            EnsureHistoryItem(ServerHelper.CurrentPressureHistoryId, null, null, currentPressure, currentFolder);

            // Humidity
            var currentRelativeHumidity = EnsureValueItem(ServerHelper.CurrentRelativeHumidityId, "Relative Humidity", null, EwsValueTypeEnum.Integer, currentFolder, "%",
                EwsValueWriteableEnum.ReadOnly, EwsValueForceableEnum.NotForceable);
            EnsureHistoryItem(ServerHelper.CurrentRelativeHumidityHistoryId, null, null, currentRelativeHumidity, currentFolder);
        }
        #endregion

        #region AddForecastPlaceHolders
        private void AddForecastPlaceHolders()
        {
            var forecast = EnsureContainerItem("Forecast");

            for (var day = 1; day <= ServerHelper.ForecastNumberOfDays; day++)
            {
                AddForecastDay(forecast, day);
            }
        }

        private void AddForecastDay(EwsContainerItem parentContainer, int dayNumber)
        {
            var containerId = string.Format(ServerHelper.ForecastDayContainerId, dayNumber);
            var parent = EnsureContainerItem(containerId, $"Day {dayNumber}", null, EwsContainerTypeEnum.Folder, parentContainer);

            // Date
            EnsureValueItem(string.Format(ServerHelper.ForecastDayDateId, dayNumber), "Date", null, EwsValueTypeEnum.DateTime, parent, null,
                EwsValueWriteableEnum.ReadOnly, EwsValueForceableEnum.NotForceable);

            // High Temp
            EnsureValueItem(string.Format(ServerHelper.ForecastDayHighTempId, dayNumber), "High Temp", null, EwsValueTypeEnum.Double, parent, "°C",
                EwsValueWriteableEnum.ReadOnly, EwsValueForceableEnum.NotForceable);

            // Low Temp
            EnsureValueItem(string.Format(ServerHelper.ForecastDayLowTempId, dayNumber), "Low Temp", null, EwsValueTypeEnum.Double, parent, "°C",
                EwsValueWriteableEnum.ReadOnly, EwsValueForceableEnum.NotForceable);

            // Pressure
            EnsureValueItem(string.Format(ServerHelper.ForecastDayPressureId, dayNumber), "Pressure", null, EwsValueTypeEnum.Double, parent, "hPa",
                EwsValueWriteableEnum.ReadOnly, EwsValueForceableEnum.NotForceable);

            // Relative Humidity
            EnsureValueItem(string.Format(ServerHelper.ForecastDayRelativeHumidityId, dayNumber), "Humidity", null, EwsValueTypeEnum.Integer, parent, "%",
                EwsValueWriteableEnum.ReadOnly, EwsValueForceableEnum.NotForceable);

            // Forecast Description
            EnsureValueItem(string.Format(ServerHelper.ForecastDayDescriptionId, dayNumber), "Forecast", null, EwsValueTypeEnum.String, parent, null,
                EwsValueWriteableEnum.ReadOnly, EwsValueForceableEnum.NotForceable);
        }
        #endregion
    }
}
