using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces
{
    public interface IChronologicalEventInfluxRepository
    {
        // Write operations
        Task<bool> WriteEventAsync(ChronologicalEventInflux chronologicalEvent);
        Task<bool> WriteEventsAsync(IEnumerable<ChronologicalEventInflux> chronologicalEvents);

        // Query operations for match analytics
        Task<IEnumerable<ChronologicalEventInflux>> GetEventsByMatchIdAsync(string matchId);
        Task<IEnumerable<ChronologicalEventInflux>> GetEventsByMatchIdAndCategoryAsync(string matchId, string eventCategory);
        Task<IEnumerable<ChronologicalEventInflux>> GetEventsByTimeRangeAsync(DateTime startTime, DateTime endTime);
        Task<IEnumerable<ChronologicalEventInflux>> GetEventsByMatchAndTimeRangeAsync(string matchId, DateTime startTime, DateTime endTime);

        // Basketball-specific analytics queries
        Task<IEnumerable<ChronologicalEventInflux>> GetEventsByPlayerAsync(string playerId, DateTime? startTime = null, DateTime? endTime = null);
        Task<IEnumerable<ChronologicalEventInflux>> GetEventsByTeamAsync(string teamId, DateTime? startTime = null, DateTime? endTime = null);
        Task<IEnumerable<ChronologicalEventInflux>> GetEventsByTypeAsync(string eventType, DateTime? startTime = null, DateTime? endTime = null);

        // Advanced queries for match flow analysis
        Task<IEnumerable<ChronologicalEventInflux>> GetScoreEvolutionAsync(string matchId);
        Task<IEnumerable<ChronologicalEventInflux>> GetEventsByPeriodAsync(string matchId, string period);
        Task<Dictionary<string, int>> GetEventCountsByTypeAsync(string matchId);

        // Delete operations (for data management)
        Task<bool> DeleteEventsByMatchIdAsync(string matchId);
        Task<bool> DeleteEventsOlderThanAsync(DateTime cutoffDate);
        
        // Saga deletion methods
        Task<int> DeleteEventsByPlayerAndTeamAsync(int playerId, int teamId);

        // Health check
        Task<bool> IsConnectedAsync();

        // Complex queries for academic requirements
        Task<IEnumerable<dynamic>> GetAdvancedMatchStatisticsAsync(string matchId);
        Task<IEnumerable<dynamic>> GetPlayerPerformanceComparisonAsync(string matchId);
        Task<IEnumerable<dynamic>> GetSeasonPlayerAveragesAsync(DateTime startTime, DateTime endTime, string? teamId = null, int minMatches = 1);

        // Report methods
        Task<IEnumerable<dynamic>> GetMatchScoringEventsInfluxReportAsync(string matchId);
        Task<IEnumerable<dynamic>> GetPlayerEventCountsInfluxReportAsync(string playerId, DateTime startTime, DateTime endTime);
        Task<IEnumerable<dynamic>> GetTeamPlayerAveragesInfluxReportAsync(DateTime startTime, DateTime endTime, string? teamId = "1");
    }
}