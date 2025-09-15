using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Infrastructure.Services;

public class MatchFinishedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MatchFinishedService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromMinutes(5); // Check every 5 minutes

    public MatchFinishedService(IServiceProvider serviceProvider, ILogger<MatchFinishedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ProcessFinishedMatches();
            await Task.Delay(_period, stoppingToken);
        }
    }

    private void ProcessFinishedMatches()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var matchRepository = scope.ServiceProvider.GetRequiredService<IMatchRepository>();

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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing finished matches");
        }
    }
}