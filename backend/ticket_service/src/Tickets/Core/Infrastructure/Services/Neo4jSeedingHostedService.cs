using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Infrastructure.Services;

public class Neo4jSeedingHostedService : BackgroundService
{
    private readonly ILogger<Neo4jSeedingHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Neo4jSeedingHostedService(
        ILogger<Neo4jSeedingHostedService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Neo4j seeding hosted service started");

            // Wait a bit for other services to start up
            await Task.Delay(2000, stoppingToken);

            using var scope = _serviceProvider.CreateScope();
            var seedingService = scope.ServiceProvider.GetRequiredService<INeo4jSeedingService>();

            var result = await seedingService.SeedDatabaseAndRemoveInitContainer();

            if (result.IsSuccess)
            {
                _logger.LogInformation("Neo4j database seeding completed successfully via hosted service");
            }
            else
            {
                _logger.LogError("Neo4j database seeding failed: {Error}", result.Error);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Neo4j seeding hosted service was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Neo4j seeding hosted service");
        }
    }
}