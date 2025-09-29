using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetMatchEventsInflux
{
    public class GetMatchEventsInfluxHandler : IRequestHandler<GetMatchEventsInfluxQuery, Result<GetMatchEventsInfluxResponse>>
    {
        private readonly IChronologicalEventInfluxRepository _chronologicalEventInfluxRepository;
        private readonly ILogger<GetMatchEventsInfluxHandler> _logger;

        public GetMatchEventsInfluxHandler(
            IChronologicalEventInfluxRepository chronologicalEventInfluxRepository,
            ILogger<GetMatchEventsInfluxHandler> logger)
        {
            _chronologicalEventInfluxRepository = chronologicalEventInfluxRepository;
            _logger = logger;
        }

        public async Task<Result<GetMatchEventsInfluxResponse>> Handle(GetMatchEventsInfluxQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var events = new List<match_service.src.Matches.Core.Domain.Entities.ChronologicalEventInflux>();

                // Execute the appropriate query based on request parameters
                if (!string.IsNullOrEmpty(request.EventCategory))
                {
                    events = (await _chronologicalEventInfluxRepository
                        .GetEventsByMatchIdAndCategoryAsync(request.MatchId, request.EventCategory)).ToList();
                }
                else if (request.StartTime.HasValue && request.EndTime.HasValue)
                {
                    events = (await _chronologicalEventInfluxRepository
                        .GetEventsByMatchAndTimeRangeAsync(request.MatchId, request.StartTime.Value, request.EndTime.Value)).ToList();
                }
                else if (!string.IsNullOrEmpty(request.Period))
                {
                    events = (await _chronologicalEventInfluxRepository
                        .GetEventsByPeriodAsync(request.MatchId, request.Period)).ToList();
                }
                else
                {
                    events = (await _chronologicalEventInfluxRepository
                        .GetEventsByMatchIdAsync(request.MatchId)).ToList();
                }

                // Get event type counts for analytics
                var eventTypeCounts = await _chronologicalEventInfluxRepository
                    .GetEventCountsByTypeAsync(request.MatchId);

                _logger.LogInformation("Retrieved {Count} events for match {MatchId}", events.Count, request.MatchId);

                return Result<GetMatchEventsInfluxResponse>.Success(new GetMatchEventsInfluxResponse
                {
                    Events = events,
                    TotalCount = events.Count,
                    MatchId = request.MatchId,
                    EventTypeCounts = eventTypeCounts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve events for match {MatchId}", request.MatchId);
                return Result<GetMatchEventsInfluxResponse>.Failure($"An error occurred: {ex.Message}")
                    .WithCode(500);
            }
        }
    }
}