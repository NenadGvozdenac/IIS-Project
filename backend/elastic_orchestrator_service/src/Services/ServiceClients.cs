using elastic_orchestrator_service.src.Models;
using System.Text.Json;
using System.Text;

namespace elastic_orchestrator_service.src.Services
{
    public class ScoutingServiceClient : IScoutingServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ScoutingServiceClient> _logger;

        public ScoutingServiceClient(HttpClient httpClient, ILogger<ScoutingServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> UpdatePlayerAsync(Player player)
        {
            try
            {
                // Transform the player data to match scouting service expectations
                var command = new ScoutingUpdatePlayerCommand
                {
                    IdPlayer = player.IdPlayer,
                    Name = player.Name,
                    Surname = player.Surname,
                    Birthday = player.Birthday?.ToString("yyyy-MM-dd"), // DateOnly format
                    Weight = player.PhysicalMetrics.FirstOrDefault()?.Weight,
                    Height = player.PhysicalMetrics.FirstOrDefault()?.Height,
                    IdNationality = GetNationalityId(player.Nationality), // Map string to ID
                    IdPosition = GetPositionId(player.Position) // Map string to ID
                };

                var json = JsonSerializer.Serialize(command, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/players/{player.IdPlayer}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully updated player {PlayerId} in scouting service", player.IdPlayer);
                    return true;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to update player {PlayerId} in scouting service. Status: {StatusCode}, Response: {Response}", 
                    player.IdPlayer, response.StatusCode, responseContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating player {PlayerId} in scouting service", player.IdPlayer);
                return false;
            }
        }

        private int? GetNationalityId(string nationality)
        {
            // Simple mapping - in a real system this would come from a lookup service
            return nationality?.ToLower() switch
            {
                "serbia" => 1,
                _ => 1 // Default to Serbia for this demo
            };
        }

        private int? GetPositionId(string position)
        {
            // Simple mapping - in a real system this would come from a lookup service
            return position?.ToLower() switch
            {
                "point guard" => 1,
                "shooting guard" => 2,
                "small forward" => 3,
                "power forward" => 4,
                "center" => 5,
                _ => 1 // Default to Point Guard
            };
        }

        public Task<bool> CompensatePlayerUpdateAsync(Player player)
        {
            try
            {
                // For compensation, we would need to restore the previous state
                // For simplicity, we'll just log this action
                // In a real scenario, you might need to store the original state before updating
                _logger.LogInformation("Compensating player update for {PlayerId} in scouting service", player.IdPlayer);
                
                // Here you would implement the actual compensation logic
                // This might involve restoring the previous player state
                
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error compensating player update for {PlayerId} in scouting service", player.IdPlayer);
                return Task.FromResult(false);
            }
        }
    }

    public class ElasticsearchServiceClient : IElasticsearchServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ElasticsearchServiceClient> _logger;

        public ElasticsearchServiceClient(HttpClient httpClient, ILogger<ElasticsearchServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> UpdatePlayerAsync(Player player)
        {
            try
            {
                var json = JsonSerializer.Serialize(player, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/players/{player.IdPlayer}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully updated player {PlayerId} in elasticsearch service", player.IdPlayer);
                    return true;
                }

                _logger.LogWarning("Failed to update player {PlayerId} in elasticsearch service. Status: {StatusCode}", 
                    player.IdPlayer, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating player {PlayerId} in elasticsearch service", player.IdPlayer);
                return false;
            }
        }

        public Task<bool> CompensatePlayerUpdateAsync(Player player)
        {
            try
            {
                // For compensation, we might need to delete the record or restore previous state
                _logger.LogInformation("Compensating player update for {PlayerId} in elasticsearch service", player.IdPlayer);
                
                // Here you would implement the actual compensation logic
                // This might involve deleting the updated record or restoring the previous state
                
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error compensating player update for {PlayerId} in elasticsearch service", player.IdPlayer);
                return Task.FromResult(false);
            }
        }
    }
}