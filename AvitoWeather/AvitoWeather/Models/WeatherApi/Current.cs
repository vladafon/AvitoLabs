using Newtonsoft.Json;

namespace AvitoWeather.Models.WeatherApi
{
    public class Current
    {
        [JsonProperty("temp_c")]
        public float TemperatureCelsius { get; set; }
    }
}
