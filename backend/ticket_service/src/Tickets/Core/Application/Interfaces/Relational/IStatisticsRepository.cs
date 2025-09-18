using ticket_service.src.Tickets.Core.Application.Features.Relational.Statistics.GetMatchStatistics;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

public interface IStatisticsRepository
{
    GetMatchStatisticsResponse? GetMatchStatistics(int matchId);
}