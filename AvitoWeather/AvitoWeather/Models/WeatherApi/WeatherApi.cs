using Newtonsoft.Json;

namespace AvitoWeather.Models.WeatherApi
{
    public class WeatherApi
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("current")]
        public Current Current{ get; set; }

        [JsonProperty("forecast")]
        public Forecast Forecast { get; set; }
    }
}
