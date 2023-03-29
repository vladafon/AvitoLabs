using Newtonsoft.Json;

namespace AvitoWeather.Models.WeatherApi
{
    public class Location
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
