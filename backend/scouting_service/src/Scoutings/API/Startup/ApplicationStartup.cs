using Microsoft.EntityFrameworkCore;
using MediatR;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Infrastructure.Repositories;
using scouting_service.src.Scoutings.Core.Infrastructure;
using scouting_service.src.Elasticsearch.Services;
using System.Reflection;
using Nest;
using Elasticsearch.Net;

namespace scouting_service.src.Scoutings.API.Startup;

public static class ApplicationStartup
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        SetupDatabases(services, configuration);
        SetupElasticsearch(services, configuration);
        SetupRepositories(services);
        SetupMediatR(services);

        return services;
    }

    private static void SetupRepositories(IServiceCollection services)
    {
        services.AddScoped<IMetricRepository, MetricRepository>();
        services.AddScoped<IMetricTypeRepository, MetricTypeRepository>();
        services.AddScoped<INationalityRepository, NationalityRepository>();
        services.AddScoped<IPhysicalMetricRepository, PhysicalMetricRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ISessionMetricRepository, SessionMetricRepository>();
        services.AddScoped<ISessionStatusRepository, SessionStatusRepository>();
        services.AddScoped<ISessionTypeRepository, SessionTypeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void SetupElasticsearch(IServiceCollection services, IConfiguration configuration)
    {
        var elasticsearchUrl = configuration.GetConnectionString("Elasticsearch") ?? "http://elasticsearch:9200";
        
        var settings = new ConnectionSettings(new Uri(elasticsearchUrl))
            .DefaultIndex("default")
            .DisableDirectStreaming()
            .OnRequestCompleted(callDetails =>
            {
                // Log Elasticsearch requests if needed
            });

        var client = new ElasticClient(settings);
        services.AddSingleton<IElasticClient>(client);

        // Register Elasticsearch services
        services.AddScoped<IElasticsearchService, ElasticsearchService>();
        services.AddScoped<IPlayerSearchService, PlayerSearchService>();
        services.AddScoped<ISessionSearchService, SessionSearchService>();
        services.AddScoped<IDataSyncService, DataSyncService>();
    }

    private static void SetupMediatR(IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
    }
    
    private static void SetupDatabases(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            "Host=postgres_db;Database=sportsdb;Username=postgres;Password=postgres;Port=5432";
        
        services.AddDbContext<ScoutingDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}