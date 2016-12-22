using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Ews.Common;
using Mongoose.Common;
using Mongoose.Common.Attributes;
using SxL.Common;
using SmartConnector.WeatherExtension.Proxy;

namespace SmartConnector.WeatherExtension
{
    /// <summary>
    /// Updates either the current weather conditions, forecast, or both.
    /// </summary>
    [ConfigurationDefaults("Weather Update Processor", "Updates either the current weather conditions, forecast, or both.")]
    public class UpdateProcessor : WeatherProcessorBase
    {
        #region ApiKey
        [Required, EncryptedString, Tooltip("Requires an API Key to run.  Obtain one for free at https://home.openweathermap.org/")]
        public string ApiKey { get; set; }
        #endregion
        #region UpdateCurrentConditions
        public bool UpdateCurrentConditions { get; set; }
        #endregion
        #region UpdateForecast
        public bool UpdateForecast { get; set; }
        #endregion

        #region Execute-Subclass - Override
        protected override IEnumerable<Prompt> Execute_Subclass()
        {
            // Make sure we can connect to an EWS Server
            if (!IsConnected) return new List<Prompt> { CreateCannotConnectPrompt() };

            // Retrieve CurrentCity ValueItem
            var currentCity = DataAdapter.ValueItems.FirstOrDefault(x => x.AlternateId == ServerHelper.CityId);
            if (currentCity == null)
            {
                return new List<Prompt>
                {
                    new Prompt
                    {
                        Message = $"{ServerHelper.CityId} ValueItem not found",
                        Severity = PromptSeverity.MayNotContinue
                    }
                };
            }

            // Perform Updates
            if (!DoUpdates(currentCity.GetValue())) ServerHelper.MakeDataServedUncertain(DataAdapter, "Unknown", LogCategory.Processor);

            // Return any issues
            return new List<Prompt>();
        }
        #endregion
        
        #region DoUpdates
        /// <summary>
        /// Retrieves and updates the Current Conditions and/or the Forecast as configured for the cityId supplied.
        /// </summary>
        /// <returns>True if successful.  False otherwise.</returns>
        /// <param name="cityId">ID of the target city in the weather web service.</param>
        private bool DoUpdates(long cityId)
        {
            if (UpdateCurrentConditions && !DoUpdateCurrentConditions(cityId)) return false;
            if (UpdateForecast && !DoUpdateForecast(cityId)) return false;
            return true;
        }
        #endregion

        #region DoUpdateCurrentConditions
        /// <summary>
        /// Updates the Current Conditions for the cityId supplied.
        /// </summary>
        /// <returns>True if successful.  False otherwise.</returns>
        /// <param name="cityId">ID of the target city in the weather web service.</param>
        private bool DoUpdateCurrentConditions(long cityId)
        {
            // Create a query builder instance
            var qb = CreateWeatherRequest(cityId, "weather");

            // Execute our request
            var currentConditions = RequestWeatherData<CurrentConditions>(qb);

            // Return successfully?
            if (currentConditions == null || currentConditions.cod == 404) return false;

            // Retrieve when we last updated the current conditions
            var currentValue = DataAdapter.ValueItems.FirstOrDefault(x => x.AlternateId == ServerHelper.CurrentLastUpdateId);

            // Lets be defensive here.  
            if (currentValue == null) return false;

            // Update when the server value is not what we have
            var serverValue = ConvertFromUnixTime(currentConditions.dt);
            if (serverValue != currentValue.GetValue() || currentValue.State == EwsValueStateEnum.Uncertain)
            {
                // Temperature
                DataAdapter.ModifyValueItemValue(ServerHelper.CurrentTemperatureId, ConvertTemperature(currentConditions.main.temp), EwsValueStateEnum.Good);

                // CurrentPressureId
                DataAdapter.ModifyValueItemValue(ServerHelper.CurrentPressureId, currentConditions.main.pressure, EwsValueStateEnum.Good);

                // CurrentRelativeHumidityId
                DataAdapter.ModifyValueItemValue(ServerHelper.CurrentRelativeHumidityId, currentConditions.main.humidity, EwsValueStateEnum.Good);

                // LastUpdated - We'll do this last
                DataAdapter.ModifyValueItemValue(currentValue, serverValue, EwsValueStateEnum.Good);
            }
            return true;
        }
        #endregion

        #region DoUpdateForecast
        /// <summary>
        /// Updates the Forecast for the cityId supplied.
        /// </summary>
        /// <returns>True if successful.  False otherwise.</returns>
        /// <param name="cityId">ID of the target city in the weather web service.</param>
        private bool DoUpdateForecast(long cityId)
        {
            // Create a query builder instance
            var qb = CreateWeatherRequest(cityId, "forecast/daily");
            qb.AddQueryParameter("cnt", ServerHelper.ForecastNumberOfDays);

            // Execute our request
            var forecast = RequestWeatherData<Forecast>(qb);

            // Return successfully?
            if (forecast == null || forecast.cod == 404) return false;

            // Update the city
            DataAdapter.ModifyValueItemValue(ServerHelper.CityNameId, forecast.city.name, EwsValueStateEnum.Good);

            var day = 1;
            foreach (var forecastDay in forecast.list)
            {
                // ForecastDayDateId 
                DataAdapter.ModifyValueItemValue(string.Format(ServerHelper.ForecastDayDateId, day), ConvertFromUnixTime(forecastDay.dt), EwsValueStateEnum.Good);

                // ForecastDayHighTempId 
                DataAdapter.ModifyValueItemValue(string.Format(ServerHelper.ForecastDayHighTempId, day), ConvertTemperature(forecastDay.temp.max), EwsValueStateEnum.Good);

                // ForecastDayLowTempId 
                DataAdapter.ModifyValueItemValue(string.Format(ServerHelper.ForecastDayLowTempId, day), ConvertTemperature(forecastDay.temp.min), EwsValueStateEnum.Good);

                // ForecastDayPressureId 
                DataAdapter.ModifyValueItemValue(string.Format(ServerHelper.ForecastDayPressureId, day), forecastDay.pressure, EwsValueStateEnum.Good);

                // ForecastDayRelativeHumidityId 
                DataAdapter.ModifyValueItemValue(string.Format(ServerHelper.ForecastDayRelativeHumidityId, day), forecastDay.humidity, EwsValueStateEnum.Good);

                // ForecastDayDescriptionId 
                var firstWeather = forecastDay.weather.FirstOrDefault();
                DataAdapter.ModifyValueItemValue(string.Format(ServerHelper.ForecastDayDescriptionId, day), firstWeather?.description, EwsValueStateEnum.Good);

                day++;
            }
            return true;
        }
        #endregion

        #region CreateWeatherRequest
        /// <summary>
        /// Returns a QueryBuilder instance for the cityId and route/action specified.
        /// </summary>
        private QueryBuilder CreateWeatherRequest(long cityId, string route)
        {
            var qb = new QueryBuilder
            {
                BaseUrl = $"http://api.openweathermap.org/data/2.5/{route}"
            };
            qb.AddQueryParameter("id", cityId);
            qb.AddQueryParameter("appid", ApiKey);
            return qb;
        }
        #endregion
        #region RequestWeatherData
        /// <summary>
        /// Execute the query supplied and return the data a TReturn
        /// </summary>
        /// <typeparam name="TReturn">Type of data to return.</typeparam>
        /// <param name="query">QueryBuilder instance of the request.</param>
        private TReturn RequestWeatherData<TReturn>(QueryBuilder query)
            where TReturn : class
        {
            using (var client = new HttpClient { BaseAddress = new Uri(query.CompleteUrl) })
            {
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // List data response.
                var response = client.GetAsync(query.CompleteUrl).Result; // Blocking call!
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return response.Content.ReadAsAsync<TReturn>().Result;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(LogCategory.Processor, ex);
                    }
                }
                else
                {
                    Logger.LogDebug(LogCategory.Processor, "Failed to connect to weather server", response.StatusCode, response.ReasonPhrase);
                }
                return null;
            }
        }
        #endregion

        #region ConvertFromUnixTime
        /// <summary>
        /// Converts seconds since Epoch to a DateTime instance
        /// </summary>
        private DateTime ConvertFromUnixTime(double value)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(value);
        }
        #endregion
        #region ConvertTemperature
        /// <summary>
        /// Converts Kelvin to Centigrade
        /// </summary>
        private double ConvertTemperature(double kelvin)
        {
            return kelvin - 273.15;
        }
        #endregion
    }
}
