using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Statistics.GetMatchStatistics;

public class GetMatchStatisticsHandler : IRequestHandler<GetMatchStatisticsQuery, Result<GetMatchStatisticsResponse>>
{
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly IMatchRepository _matchRepository;

    public GetMatchStatisticsHandler(IStatisticsRepository statisticsRepository, IMatchRepository matchRepository)
    {
        _statisticsRepository = statisticsRepository;
        _matchRepository = matchRepository;
    }

    public Task<Result<GetMatchStatisticsResponse>> Handle(GetMatchStatisticsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // First check if match exists
            var match = _matchRepository.GetById(request.MatchId);
            if (match == null)
            {
                return Task.FromResult(Result<GetMatchStatisticsResponse>.Failure("Match not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Check if match has finished
            if (match.ScheduledAt > DateTime.UtcNow)
            {
                return Task.FromResult(Result<GetMatchStatisticsResponse>.Failure("Statistics are not available for matches that haven't finished yet")
                    .WithCode((int)ResultCode.BadRequest));
            }

            var statistics = _statisticsRepository.GetMatchStatistics(request.MatchId);
            
            if (statistics == null)
            {
                return Task.FromResult(Result<GetMatchStatisticsResponse>.Failure("No statistics data available for this match")
                    .WithCode((int)ResultCode.NotFound));
            }

            return Task.FromResult(Result<GetMatchStatisticsResponse>.Success(statistics));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetMatchStatisticsResponse>.Failure($"An error occurred while retrieving match statistics: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}