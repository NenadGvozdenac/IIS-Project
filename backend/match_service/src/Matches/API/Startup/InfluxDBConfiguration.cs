using InfluxDB.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure.Repositories;

namespace match_service.src.Matches.API.Startup
{
    public static class InfluxDBConfiguration
    {
        public static IServiceCollection ConfigureInfluxDB(this IServiceCollection services, IConfiguration configuration)
        {
            var influxDbUrl = configuration["InfluxDB:Url"] ?? "http://influxdb:8086";
            var influxDbToken = configuration["InfluxDB:Token"] ?? "basketball-events-token-2024";

            // Register InfluxDB client as singleton
            services.AddSingleton<InfluxDBClient>(provider =>
            {
                var options = new InfluxDBClientOptions.Builder()
                    .Url(influxDbUrl)
                    .AuthenticateToken(influxDbToken)
                    .Build();

                return new InfluxDBClient(options);
            });

            // Register repository
            services.AddScoped<IChronologicalEventInfluxRepository, ChronologicalEventInfluxRepository>();

            return services;
        }
    }
}