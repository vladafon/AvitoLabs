using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace AvitoWeather
{
    /// <summary>
    ///     Делает запросы через httpClient
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RequestMaker<T> where T : class
    {
        /// <summary>
        ///     Делает Get запрос по указанному url с параметрами
        /// </summary>
        /// <param name="url">Полный url</param>
        /// <param name="query">Словарь get параметров</param>
        /// <returns></returns>
        public async Task<T> Get(string url, Dictionary<string, string> query)
        {
            var client = new HttpClient();

            var uri = QueryHelpers.AddQueryString(url, query);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri)
                
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(body);
            }
        }
    }
}
