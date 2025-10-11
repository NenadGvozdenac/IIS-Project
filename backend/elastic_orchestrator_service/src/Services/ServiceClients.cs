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

        public async Task<Player?> GetPlayerAsync(int playerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/players/{playerId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var player = JsonSerializer.Deserialize<Player>(jsonContent, new JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });
                    
                    _logger.LogInformation("Successfully retrieved player {PlayerId} from scouting service", playerId);
                    return player;
                }

                _logger.LogWarning("Failed to retrieve player {PlayerId} from scouting service. Status: {StatusCode}", 
                    playerId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving player {PlayerId} from scouting service", playerId);
                return null;
            }
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

        public async Task<bool> CompensatePlayerUpdateAsync(Player originalPlayer)
        {
            try
            {
                _logger.LogInformation("Compensating player update for {PlayerId} in scouting service", originalPlayer.IdPlayer);
                
                // Transform the original player data to match scouting service expectations
                var command = new ScoutingUpdatePlayerCommand
                {
                    IdPlayer = originalPlayer.IdPlayer,
                    Name = originalPlayer.Name,
                    Surname = originalPlayer.Surname,
                    Birthday = originalPlayer.Birthday?.ToString("yyyy-MM-dd"),
                    Weight = originalPlayer.PhysicalMetrics.FirstOrDefault()?.Weight,
                    Height = originalPlayer.PhysicalMetrics.FirstOrDefault()?.Height,
                    IdNationality = GetNationalityId(originalPlayer.Nationality),
                    IdPosition = GetPositionId(originalPlayer.Position)
                };

                var json = JsonSerializer.Serialize(command, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Restore the original player state
                var response = await _httpClient.PutAsync($"/api/players/{originalPlayer.IdPlayer}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully compensated player {PlayerId} in scouting service", originalPlayer.IdPlayer);
                    return true;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to compensate player {PlayerId} in scouting service. Status: {StatusCode}, Response: {Response}", 
                    originalPlayer.IdPlayer, response.StatusCode, responseContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error compensating player update for {PlayerId} in scouting service", originalPlayer.IdPlayer);
                return false;
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

        public async Task<Player?> GetPlayerAsync(int playerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/players/{playerId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var player = JsonSerializer.Deserialize<Player>(jsonContent, new JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });
                    
                    _logger.LogInformation("Successfully retrieved player {PlayerId} from elasticsearch service", playerId);
                    return player;
                }

                _logger.LogWarning("Failed to retrieve player {PlayerId} from elasticsearch service. Status: {StatusCode}", 
                    playerId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving player {PlayerId} from elasticsearch service", playerId);
                return null;
            }
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

        public async Task<bool> CompensatePlayerUpdateAsync(Player originalPlayer)
        {
            try
            {
                _logger.LogInformation("Compensating player update for {PlayerId} in elasticsearch service", originalPlayer.IdPlayer);
                
                // Restore the original player state in Elasticsearch
                var json = JsonSerializer.Serialize(originalPlayer, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/players/{originalPlayer.IdPlayer}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully compensated player {PlayerId} in elasticsearch service", originalPlayer.IdPlayer);
                    return true;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to compensate player {PlayerId} in elasticsearch service. Status: {StatusCode}, Response: {Response}", 
                    originalPlayer.IdPlayer, response.StatusCode, responseContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error compensating player update for {PlayerId} in elasticsearch service", originalPlayer.IdPlayer);
                return false;
            }
        }
    }
}