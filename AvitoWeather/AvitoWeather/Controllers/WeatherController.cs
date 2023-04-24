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
using AvitoWeather.Core;
using Ardalis.GuardClauses;
using Newtonsoft.Json;

namespace AvitoWeather.Controllers
{
    [ApiController]
    [Route("v1")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly Settings _settings;
        private readonly RequestMaker<WeatherApi> _requestMaker;
        private readonly IWeatherService _weatherService;

        private const string CelsiusUnit = "celsius";

        public WeatherController(ILogger<WeatherController> logger,
            IOptionsMonitor<Settings> settingsMonitor,
            RequestMaker<WeatherApi> requestMaker,
            IWeatherService weatherService)
        {
            _logger = logger;
            _requestMaker = requestMaker;
            _weatherService = weatherService;

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

            var cachedWeatherJson = await _weatherService.GetFromCache(city);

            if (cachedWeatherJson != null)
            {
                var cachedWeather = JsonConvert.DeserializeObject<WeatherData>(cachedWeatherJson);

                return Ok(cachedWeather);
            }

            var url = "current.json";
            var fullUrl = GetFullUrl(url);

            var query = GetQueryWithKey();
            query.Add("q", city);

            var result = await _requestMaker.Get(fullUrl, query);

            _logger.LogInformation("Текущая погода получена успешно");

            var weatherData = new WeatherData
            {
                City = result.Location.Name,
                Temperature = result.Current.TemperatureCelsius,
                Unit = CelsiusUnit
            };

            var weatherDataJson = JsonConvert.SerializeObject(weatherData);

            await _weatherService.SaveToCache(city, weatherDataJson);

            return Ok(weatherData);
        }

        [HttpPut]
        [Route("put")]
        public async Task<IActionResult> PutWeather([FromHeader(Name = "Own-Auth-UserName")] string ownAuthUserName, [FromBody] WeatherData weather)
        {
            //if (!IsAuth(ownAuthUserName))
            //{
            //    return StatusCode(403);
            //}

            Guard.Against.Null(weather, nameof(weather));
            Guard.Against.NullOrWhiteSpace(weather.City, nameof(weather.City));
            Guard.Against.NullOrWhiteSpace(weather.Unit, nameof(weather.Unit));

            var weatherJson = JsonConvert.SerializeObject(weather);

            await _weatherService.SaveToCache(weather.City, weatherJson);

            _logger.LogInformation("Данные о погоде успешна добавлены");

            return Created($"{Request.Scheme}://{Request.Host}{Request.PathBase}", weather);
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
