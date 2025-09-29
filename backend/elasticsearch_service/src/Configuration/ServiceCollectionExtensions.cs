using Nest;
using elasticsearch_service.src.Services;

namespace elasticsearch_service.src.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var elasticsearchUri = configuration.GetValue<string>("Elasticsearch:Uri");
            
            var settings = new ConnectionSettings(new Uri(elasticsearchUri!))
                .DefaultIndex("default")
                .DisableDirectStreaming();

            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);
            services.AddScoped<IElasticsearchService, ElasticsearchService>();

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