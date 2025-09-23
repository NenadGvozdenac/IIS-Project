using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure.Services;

public class MatchFinishedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MatchFinishedService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromMinutes(5); // Check every 5 minutes
    private const int MaxRetries = 30;
    private const int RetryDelayMs = 2000;

    public MatchFinishedService(IServiceProvider serviceProvider, ILogger<MatchFinishedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for database to be available before starting the service
        using var scope = _serviceProvider.CreateScope();
        var databaseConnectionService = scope.ServiceProvider.GetRequiredService<IDatabaseConnectionService>();
        
        _logger.LogInformation("MatchFinishedService starting, waiting for database connection...");
        var databaseReady = await databaseConnectionService.WaitForDatabaseConnection(MaxRetries, RetryDelayMs);
        
        if (!databaseReady)
        {
            _logger.LogError("MatchFinishedService failed to start due to database connection issues");
            return;
        }
        
        _logger.LogInformation("MatchFinishedService started successfully, database connection established");

        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessFinishedMatches();
            await Task.Delay(_period, stoppingToken);
        }
    }

    private async Task ProcessFinishedMatches()
    {
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var matchRepository = scope.ServiceProvider.GetRequiredService<IMatchRepository>();

                _logger.LogDebug($"Attempting to process finished matches (attempt {attempt}/{MaxRetries})...");

                // Find all matches that have finished but still have tickets_for_sale = true
                var finishedMatches = matchRepository.GetAll()
                    .Where(m => m.ScheduledAt <= DateTime.UtcNow && m.TicketsForSale)
                    .ToList();

                if (finishedMatches.Any())
                {
                    _logger.LogInformation("Found {Count} finished matches to process", finishedMatches.Count);

                    foreach (var match in finishedMatches)
                    {
                        // Update tickets_for_sale to false - this will trigger the database trigger
                        match.TicketsForSale = false;
                        matchRepository.Update(match);
                        
                        _logger.LogInformation("Updated match {MatchId} ({MatchName}) - tickets_for_sale set to false", 
                            match.IdMatch, match.Name);
                    }
                }
                else
                {
                    _logger.LogDebug("No finished matches to process");
                }

                // If we reach here, the operation was successful
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to process finished matches (attempt {attempt}/{MaxRetries}): {ex.Message}");
                
                if (attempt == MaxRetries)
                {
                    _logger.LogError(ex, "Error occurred while processing finished matches after maximum retries");
                    return;
                }
                
                await Task.Delay(RetryDelayMs);
            }
        }
    }
}