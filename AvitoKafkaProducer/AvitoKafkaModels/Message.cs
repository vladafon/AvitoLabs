using Newtonsoft.Json;

namespace AvitoKafkaProducer.Models
{
    public class Message
    {
        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }
    }
}
