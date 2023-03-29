using Newtonsoft.Json;

namespace AvitoWeather.Models.WeatherApi
{
    public class Day
    {
        [JsonProperty("avgtemp_c")]
        public float AverageTemperatureCelsius { get; set; }
    }
}
