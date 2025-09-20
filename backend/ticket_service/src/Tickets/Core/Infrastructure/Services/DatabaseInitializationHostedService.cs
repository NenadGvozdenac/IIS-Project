using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure.Services;

public class DatabaseInitializationHostedService : BackgroundService
{
    private readonly ILogger<DatabaseInitializationHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const int MaxRetries = 30;
    private const int RetryDelayMs = 2000;

    public DatabaseInitializationHostedService(
        ILogger<DatabaseInitializationHostedService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Database initialization service started");

            using var scope = _serviceProvider.CreateScope();
            var databaseConnectionService = scope.ServiceProvider.GetRequiredService<IDatabaseConnectionService>();

            var databaseReady = await databaseConnectionService.WaitForDatabaseConnection(MaxRetries, RetryDelayMs);

            if (databaseReady)
            {
                _logger.LogInformation("Database initialization completed successfully - PostgreSQL database is available");
            }
            else
            {
                _logger.LogError("Database initialization failed - PostgreSQL database is not available after maximum retries");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Database initialization service was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in database initialization service");
        }
    }
}