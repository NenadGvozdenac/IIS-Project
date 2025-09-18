using Neo4j.Driver;
using System.Diagnostics;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Infrastructure.Services;

public class Neo4jSeedingService : INeo4jSeedingService
{
    private readonly ILogger<Neo4jSeedingService> _logger;
    private readonly IConfiguration _configuration;

    public Neo4jSeedingService(ILogger<Neo4jSeedingService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<Result> SeedDatabaseAndRemoveInitContainer()
    {
        try
        {
            _logger.LogInformation("Starting Neo4j database seeding process...");

            var seedResult = await ExecuteNeo4jSeedingScript();
            if (!seedResult)
            {
                return Result.Failure("Failed to execute Neo4j seeding script").WithCode((int)ResultCode.InternalServerError);
            }

            _logger.LogInformation("Neo4j database seeding completed successfully");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during Neo4j seeding process");
            return Result.Failure($"Error during Neo4j seeding: {ex.Message}").WithCode((int)ResultCode.InternalServerError);
        }
    }

    private async Task<bool> ExecuteNeo4jSeedingScript()
    {
        try
        {
            var neo4jUri = _configuration.GetConnectionString("Neo4j") ?? "bolt://neo4j:7687";
            var neo4jUser = _configuration["Neo4j:Username"] ?? "neo4j";
            var neo4jPassword = _configuration["Neo4j:Password"] ?? "password";

            using var driver = GraphDatabase.Driver(neo4jUri, AuthTokens.Basic(neo4jUser, neo4jPassword));
            using var session = driver.AsyncSession();

            var scriptPath = "/database/graph_database/neo4j_init.cql";
            if (!File.Exists(scriptPath))
            {
                _logger.LogError($"Neo4j init script not found at {scriptPath}");
                return false;
            }

            var script = await File.ReadAllTextAsync(scriptPath);
            
            var statements = script.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var statement in statements)
            {
                var trimmedStatement = statement.Trim();
                if (string.IsNullOrEmpty(trimmedStatement))
                    continue;

                _logger.LogDebug($"Executing Neo4j statement: {trimmedStatement[..Math.Min(100, trimmedStatement.Length)]}...");
                
                await session.RunAsync(trimmedStatement);
            }

            _logger.LogInformation("Neo4j seeding script executed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute Neo4j seeding script");
            return false;
        }
    }
}