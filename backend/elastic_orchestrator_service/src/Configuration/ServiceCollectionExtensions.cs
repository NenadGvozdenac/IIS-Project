using elastic_orchestrator_service.src.Services;
using Microsoft.Extensions.DependencyInjection;

namespace elastic_orchestrator_service.src.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Scouting Service Client
            services.AddHttpClient<IScoutingServiceClient, ScoutingServiceClient>(client =>
            {
                var baseUrl = configuration.GetValue<string>("Services:ScoutingService");
                client.BaseAddress = new Uri(baseUrl!);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Add Elasticsearch Service Client
            services.AddHttpClient<IElasticsearchServiceClient, ElasticsearchServiceClient>(client =>
            {
                var baseUrl = configuration.GetValue<string>("Services:ElasticsearchService");
                client.BaseAddress = new Uri(baseUrl!);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            return services;
        }

        public static IServiceCollection AddSagaServices(this IServiceCollection services)
        {
            services.AddScoped<ISagaOrchestrator, SagaOrchestrator>();
            return services;
        }

        public static IServiceCollection ConfigureCors(this IServiceCollection services, string corsPolicy)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicy, policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }
    }
}