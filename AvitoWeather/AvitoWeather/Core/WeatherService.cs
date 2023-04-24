using Ardalis.GuardClauses;
using AvitoWeather.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Threading.Tasks;

namespace AvitoWeather.Core
{
    public class WeatherService : IWeatherService
    {
        private readonly IRedisClient _redisClient;
        private readonly ILogger<WeatherService> _logger;
        public WeatherService(IRedisClient redisClient, ILogger<WeatherService> logger)
        {
            _redisClient = redisClient;
            _logger = logger;
        }

        public async Task<string> GetFromCache(string city)
        {
            Guard.Against.NullOrWhiteSpace(city, nameof(city));

            try
            {
                var result = await _redisClient.GetDefaultDatabase().GetAsync<string>(city);

                if (result != null)
                { 
                    _logger.LogInformation($"Получена запись по ключу {city}"); 
                }
                else
                {
                    _logger.LogInformation($"Запись по ключу {city} не найдена");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка получения данных по ключу {city}: {ex}");
                return null;
            }
        }

        public async Task SaveToCache(string city, string weatherJson)
        {
            Guard.Against.NullOrWhiteSpace(city, nameof(city));
            Guard.Against.NullOrWhiteSpace(weatherJson, nameof(weatherJson));
            Guard.Against.InvalidInput<string>(weatherJson, nameof(weatherJson), (str) => 
            {
                try
                {
                    var weather = JsonConvert.DeserializeObject<WeatherData>(str);
                    return true;
                }
                catch
                {
                    return false;
                }
            });


            try
            {
                var added = await _redisClient.GetDefaultDatabase().AddAsync(city, weatherJson, DateTimeOffset.Now.AddHours(1));

                if (added)
                { 
                    _logger.LogInformation($"Добавлена запись по ключу {city}"); 
                }
                else
                {
                    _logger.LogError($"Ошибка добавления записи по ключу {city}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка сохранения данных по ключу {city}: {ex}");
            }
        }
    }
}
