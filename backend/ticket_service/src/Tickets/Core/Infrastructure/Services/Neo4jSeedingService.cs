using Neo4j.Driver;
using System.Diagnostics;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Infrastructure.Services;

public class Neo4jSeedingService : INeo4jSeedingService
{
    private readonly ILogger<Neo4jSeedingService> _logger;
    private readonly IConfiguration _configuration;
    private const int MaxRetries = 30;
    private const int RetryDelayMs = 2000;

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

            // Wait for Neo4j to be ready
            var connectionResult = await WaitForNeo4jConnection();
            if (!connectionResult.IsSuccess)
            {
                return connectionResult;
            }

            // Check if database is already seeded
            var seedCheckResult = await CheckIfDatabaseIsSeeded();
            if (!seedCheckResult.IsSuccess)
            {
                return seedCheckResult;
            }

            if (seedCheckResult.Value)
            {
                _logger.LogInformation("Neo4j database is already seeded, skipping seeding process");
                return Result.Success();
            }

            // Execute seeding
            var seedResult = await ExecuteNeo4jSeedingScript();
            if (!seedResult.IsSuccess)
            {
                return seedResult;
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

    private async Task<Result> WaitForNeo4jConnection()
    {
        var neo4jUri = _configuration["Neo4j:Uri"] ?? "bolt://neo4j:7687";
        var neo4jUser = _configuration["Neo4j:Username"] ?? "neo4j";
        var neo4jPassword = _configuration["Neo4j:Password"] ?? "password";

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                _logger.LogInformation($"Attempting to connect to Neo4j (attempt {attempt}/{MaxRetries})...");
                
                using var driver = GraphDatabase.Driver(neo4jUri, AuthTokens.Basic(neo4jUser, neo4jPassword));
                using var session = driver.AsyncSession();
                
                // Test connection with a simple query
                await session.RunAsync("RETURN 1");
                
                _logger.LogInformation("Successfully connected to Neo4j database");
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to connect to Neo4j (attempt {attempt}/{MaxRetries}): {ex.Message}");
                
                if (attempt == MaxRetries)
                {
                    _logger.LogError("Max connection attempts reached. Neo4j is not available.");
                    return Result.Failure("Failed to connect to Neo4j after maximum retries").WithCode((int)ResultCode.InternalServerError);
                }
                
                await Task.Delay(RetryDelayMs);
            }
        }
        
        return Result.Failure("Unexpected error while connecting to Neo4j").WithCode((int)ResultCode.InternalServerError);
    }

    private async Task<Result<bool>> CheckIfDatabaseIsSeeded()
    {
        try
        {
            var neo4jUri = _configuration["Neo4j:Uri"] ?? "bolt://neo4j:7687";
            var neo4jUser = _configuration["Neo4j:Username"] ?? "neo4j";
            var neo4jPassword = _configuration["Neo4j:Password"] ?? "password";

            using var driver = GraphDatabase.Driver(neo4jUri, AuthTokens.Basic(neo4jUser, neo4jPassword));
            using var session = driver.AsyncSession();

            // Check if there are any Customer nodes (our seed data includes customers)
            var result = await session.RunAsync("MATCH (c:Customer) RETURN count(c) as customerCount");
            var record = await result.SingleAsync();
            var customerCount = record["customerCount"].As<long>();

            _logger.LogInformation($"Found {customerCount} customers in Neo4j database");
            
            // If we have customers, assume the database is seeded
            return Result<bool>.Success(customerCount > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if database is seeded");
            return Result<bool>.Failure($"Failed to check database state: {ex.Message}").WithCode((int)ResultCode.InternalServerError);
        }
    }

    private async Task<Result> ExecuteNeo4jSeedingScript()
    {
        try
        {
            var neo4jUri = _configuration["Neo4j:Uri"] ?? "bolt://neo4j:7687";
            var neo4jUser = _configuration["Neo4j:Username"] ?? "neo4j";
            var neo4jPassword = _configuration["Neo4j:Password"] ?? "password";

            using var driver = GraphDatabase.Driver(neo4jUri, AuthTokens.Basic(neo4jUser, neo4jPassword));
            using var session = driver.AsyncSession();

            var scriptPath = "/database/graph_database/neo4j_init.cql";
            if (!File.Exists(scriptPath))
            {
                _logger.LogError($"Neo4j init script not found at {scriptPath}");
                return Result.Failure($"Seeding script not found at {scriptPath}").WithCode((int)ResultCode.NotFound);
            }

            var script = await File.ReadAllTextAsync(scriptPath);
            
            // Split script into individual statements (more sophisticated parsing)
            var statements = SplitCypherScript(script);
            
            _logger.LogInformation($"Executing {statements.Count} Neo4j statements...");

            for (int i = 0; i < statements.Count; i++)
            {
                var statement = statements[i];
                if (string.IsNullOrWhiteSpace(statement))
                    continue;

                try
                {
                    _logger.LogDebug($"Executing statement {i + 1}/{statements.Count}: {statement.Substring(0, Math.Min(100, statement.Length))}...");
                    await session.RunAsync(statement);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to execute statement {i + 1}: {statement.Substring(0, Math.Min(200, statement.Length))}...");
                    return Result.Failure($"Failed to execute seeding statement {i + 1}: {ex.Message}").WithCode((int)ResultCode.InternalServerError);
                }
            }

            _logger.LogInformation("Neo4j seeding script executed successfully");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute Neo4j seeding script");
            return Result.Failure($"Failed to execute seeding script: {ex.Message}").WithCode((int)ResultCode.InternalServerError);
        }
    }

    private List<string> SplitCypherScript(string script)
    {
        var statements = new List<string>();
        var lines = script.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var currentStatement = new List<string>();

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            // Skip comments and empty lines
            if (trimmedLine.StartsWith("//") || string.IsNullOrWhiteSpace(trimmedLine))
                continue;
            
            currentStatement.Add(line);
            
            // If line ends with semicolon, it's the end of a statement
            if (trimmedLine.EndsWith(";"))
            {
                var statement = string.Join("\n", currentStatement).Trim();
                if (!string.IsNullOrWhiteSpace(statement))
                {
                    // Remove the trailing semicolon as Neo4j doesn't need it
                    statements.Add(statement.TrimEnd(';').Trim());
                }
                currentStatement.Clear();
            }
        }
        
        // Add any remaining statement
        if (currentStatement.Count > 0)
        {
            var statement = string.Join("\n", currentStatement).Trim();
            if (!string.IsNullOrWhiteSpace(statement))
            {
                statements.Add(statement);
            }
        }
        
        return statements;
    }
}