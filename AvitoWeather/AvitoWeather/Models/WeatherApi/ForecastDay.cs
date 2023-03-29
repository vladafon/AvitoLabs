using Newtonsoft.Json;

namespace AvitoWeather.Models.WeatherApi
{
    public class ForecastDay
    {
        [JsonProperty("day")]
        public Day Day { get; set; }
    }
}
