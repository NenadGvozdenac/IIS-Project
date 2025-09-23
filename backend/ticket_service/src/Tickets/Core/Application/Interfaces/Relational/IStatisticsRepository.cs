using ticket_service.src.Tickets.Core.Application.Features.Relational.Statistics.GetMatchStatistics;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

public interface IStatisticsRepository
{
    GetMatchStatisticsResponse? GetMatchStatistics(int matchId);
    Task<MatchSummaryReportResponse> GetMatchSummaryReportAsync();
}