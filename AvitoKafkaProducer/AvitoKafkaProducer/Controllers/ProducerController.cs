using AvitoKafkaProducer.Models;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace AvitoKafkaProducer.Controllers
{
    [ApiController]
    [Route("v1/producer")]
    public class ProducerController : ControllerBase
    {
        private readonly ILogger<ProducerController> _logger;
        private ProducerConfig _producerConfig;
        private readonly Settings _settings;

        public ProducerController(ILogger<ProducerController> logger, ProducerConfig producerConfig, IOptionsMonitor<Settings> settingsMonitor)
        {
            _producerConfig = producerConfig;
            _settings = settingsMonitor.CurrentValue;
            _logger = logger;
        }


        [HttpGet("send-message")]
        public async Task<ActionResult> SendMessage()
        {
            var message = new Message { Type = MessageTypes.Message };

            var serializedData = JsonConvert.SerializeObject(message);

            using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                await producer.ProduceAsync(_settings.KafkaTopicName, new Message<Null, string> { Value = serializedData });
                producer.Flush(TimeSpan.FromSeconds(10));
            }

            _logger.LogInformation("Сообщение успешно отправлено");

            return Ok("Сообщение успешно отправлено");
        }

        [HttpGet("send-error")]
        public async Task<ActionResult> SendError()
        {
            var error = new Message { Type = MessageTypes.Error };

            var serializedData = JsonConvert.SerializeObject(error);

            using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                await producer.ProduceAsync(_settings.KafkaTopicName, new Message<Null, string> { Value = serializedData });
                producer.Flush(TimeSpan.FromSeconds(10));
            }

            _logger.LogInformation("Сообщение с ошибкой успешно отправлено");

            return Ok("Сообщение с ошибкой успешно отправлено");
        }
    }
}
