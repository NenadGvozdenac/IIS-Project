using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.Core.Infrastructure;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure.Services;

public class DatabaseConnectionService : IDatabaseConnectionService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseConnectionService> _logger;

    public DatabaseConnectionService(IServiceProvider serviceProvider, ILogger<DatabaseConnectionService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<bool> WaitForDatabaseConnection(int maxRetries = 30, int retryDelayMs = 2000)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                _logger.LogInformation($"Attempting to connect to PostgreSQL database (attempt {attempt}/{maxRetries})...");
                
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<TicketDbContext>();
                
                // Test connection with a simple query
                await context.Database.ExecuteSqlRawAsync("SELECT 1");
                
                _logger.LogInformation("Successfully connected to PostgreSQL database");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to connect to PostgreSQL database (attempt {attempt}/{maxRetries}): {ex.Message}");
                
                if (attempt == maxRetries)
                {
                    _logger.LogError("Max connection attempts reached. PostgreSQL database is not available.");
                    return false;
                }
                
                await Task.Delay(retryDelayMs);
            }
        }
        
        return false;
    }

    public async Task<bool> TestDatabaseConnection()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<TicketDbContext>();
            
            // Test connection with a simple query
            await context.Database.ExecuteSqlRawAsync("SELECT 1");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogDebug($"Database connection test failed: {ex.Message}");
            return false;
        }
    }
}