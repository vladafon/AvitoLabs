using Newtonsoft.Json;

namespace AvitoWeather.Models
{
    public class WeatherData
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("temperature")]
        public float Temperature { get; set; }
    }
}
