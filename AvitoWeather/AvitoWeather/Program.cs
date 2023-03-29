using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvitoWeather
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    var listenPortString = Environment.GetEnvironmentVariable("LISTEN_PORT");

                    var listenPort = Convert.ToInt32(listenPortString);

                    //var listenPortSslString = Environment.GetEnvironmentVariable("LISTEN_PORT_SSL");

                    //var listenPortSsl = Convert.ToInt32(listenPortSslString);

                    webBuilder.UseUrls
                    (
                        new string[] { $"http://+:{listenPort}"/*, $"https://+:{listenPortSsl}"*/ }
                        );
                });
    }
}
