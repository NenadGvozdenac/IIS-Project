using ticket_service.src.Tickets.Core.Application.Features.Statistics.GetMatchStatistics;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface IStatisticsRepository
{
    GetMatchStatisticsResponse? GetMatchStatistics(int matchId);
}