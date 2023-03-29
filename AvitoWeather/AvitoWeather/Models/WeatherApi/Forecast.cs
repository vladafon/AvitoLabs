using Newtonsoft.Json;
using System.Collections.Generic;

namespace AvitoWeather.Models.WeatherApi
{
    public class Forecast
    {
        [JsonProperty("forecastday")]
        public List<ForecastDay> ForecastDays { get; set; }
    }
}
