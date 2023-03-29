using AvitoWeather.Models;
using AvitoWeather.Models.WeatherApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcAuthClient;

namespace AvitoWeather.Controllers
{
    [ApiController]
    [Route("v1")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly Settings _settings;
        private readonly RequestMaker<WeatherApi> _requestMaker;

        private const string CelsiusUnit = "celsius";

        public WeatherController(ILogger<WeatherController> logger,
            IOptionsMonitor<Settings> settingsMonitor,
            RequestMaker<WeatherApi> requestMaker)
        {
            _logger = logger;
            _requestMaker = requestMaker;

            _settings = settingsMonitor.CurrentValue;
        }

        [HttpGet]
        [Route("forecast")]
        public async Task<IActionResult> GetForecast([FromHeader(Name = "Own-Auth-UserName")] string ownAuthUserName, [FromQuery] string city, [FromQuery] int dt)
        {
            //if (!IsAuth(ownAuthUserName))
            //{
            //    return StatusCode(403);
            //}

            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(dt);

            var now = DateTimeOffset.Now;

            if (dateTimeOffset <= now || dateTimeOffset >= now.AddDays(14))
            {
                var errorText = "Дата не соответствует диапазону";
                _logger.LogError(errorText);
                return BadRequest(errorText);
            }

            var url = "forecast.json";
            var fullUrl = GetFullUrl(url);

            var query = GetQueryWithKey();
            query.Add("q", city);
            query.Add("unixdt", dt.ToString());

            var result = await _requestMaker.Get(fullUrl, query);

            _logger.LogInformation("Прогноз погоды получен успешно");

            return Ok(new WeatherData
            {
                City = result.Location.Name,
                Temperature = result.Forecast.ForecastDays[0].Day.AverageTemperatureCelsius,
                Unit = CelsiusUnit
            });
        }

        [HttpGet]
        [Route("current")]
        public async Task<IActionResult> GetCurrentWeather([FromHeader(Name = "Own-Auth-UserName")] string ownAuthUserName, [FromQuery] string city)
        {
            //if (!IsAuth(ownAuthUserName))
            //{
            //    return StatusCode(403);
            //}

            var url = "current.json";
            var fullUrl = GetFullUrl(url);

            var query = GetQueryWithKey();
            query.Add("q", city);

            var result = await _requestMaker.Get(fullUrl, query);

            _logger.LogInformation("Текущая погода получена успешно");

            return Ok(new WeatherData
            {
                City = result.Location.Name,
                Temperature = result.Current.TemperatureCelsius,
                Unit = CelsiusUnit
            });
        }

        private string GetFullUrl(string methodUrl)
        {
            Uri baseUri = new Uri(_settings.ApiUrl);
            var myUri = new Uri(baseUri, methodUrl);

            return myUri.ToString();
        }

        private Dictionary<string, string> GetQueryWithKey()
        {
            var query = new Dictionary<string, string>();
            query.Add("key", _settings.ApiKey);

            return query;
        }

        private bool IsAuth(string userName)
        {
            // The port number must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress(_settings.AuthServiceUrl);
            var client = new AuthService.AuthServiceClient(channel);
            var reply = client.IsAuth(
                              new Request { UserName = userName });

            return reply.IsAuthResult;
        }
    }
}
