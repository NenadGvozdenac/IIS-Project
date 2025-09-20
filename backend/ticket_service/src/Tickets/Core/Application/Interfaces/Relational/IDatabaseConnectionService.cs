namespace ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

public interface IDatabaseConnectionService
{
    Task<bool> WaitForDatabaseConnection(int maxRetries = 30, int retryDelayMs = 2000);
    Task<bool> TestDatabaseConnection();
}