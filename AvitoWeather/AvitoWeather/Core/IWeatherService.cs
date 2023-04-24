using System.Threading.Tasks;

namespace AvitoWeather.Core
{
    public interface IWeatherService
    {
        /// <summary>
        ///     Сохраняет погоду в кеш
        /// </summary>
        /// <param name="city">Уникальный ключ - город</param>
        /// <param name="weatherJson">Json с информацией о погоде</param>
        Task SaveToCache(string city, string weatherJson);

        /// <summary>
        ///     Получает погоду из кеша, при ее наличии
        /// </summary>
        /// <param name="city">Уникальный ключ - город</param>
        /// <returns>Json с информацией о погоде, при наличии. При отсутствии вовзращает <see langword="null" />.</returns>
        Task<string> GetFromCache(string city);
    }
}
