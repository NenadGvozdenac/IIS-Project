using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.CreateEventInflux
{
    public class CreateEventInfluxHandler : IRequestHandler<CreateEventInfluxCommand, Result<CreateEventInfluxResponse>>
    {
        private readonly IChronologicalEventInfluxRepository _chronologicalEventInfluxRepository;
        private readonly ILogger<CreateEventInfluxHandler> _logger;

        public CreateEventInfluxHandler(
            IChronologicalEventInfluxRepository chronologicalEventInfluxRepository,
            ILogger<CreateEventInfluxHandler> logger)
        {
            _chronologicalEventInfluxRepository = chronologicalEventInfluxRepository;
            _logger = logger;
        }

        public async Task<Result<CreateEventInfluxResponse>> Handle(CreateEventInfluxCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var chronologicalEvent = new ChronologicalEventInflux
                {
                    Timestamp = request.Timestamp ?? DateTime.UtcNow,
                    MatchId = request.MatchId,
                    EventCategory = request.EventCategory,
                    EventType = request.EventType,
                    Period = request.Period,
                    PeriodTime = request.PeriodTime,
                    TeamId = request.TeamId,
                    PlayerId = request.PlayerId,
                    PlayerName = request.PlayerName,
                    EventId = request.EventId,
                    Notes = request.Notes,
                    OurPoints = request.OurPoints,
                    OpponentPoints = request.OpponentPoints,
                    PointDifference = request.OurPoints.HasValue && request.OpponentPoints.HasValue 
                        ? request.OurPoints.Value - request.OpponentPoints.Value 
                        : null,
                    CreationTime = DateTime.UtcNow
                };

                var success = await _chronologicalEventInfluxRepository.WriteEventAsync(chronologicalEvent);

                if (success)
                {
                    _logger.LogInformation("Successfully created InfluxDB event {EventId} for match {MatchId}", 
                        request.EventId, request.MatchId);

                    return Result<CreateEventInfluxResponse>.Success(new CreateEventInfluxResponse
                    {
                        Success = true,
                        Message = "Event successfully stored in InfluxDB",
                        Timestamp = chronologicalEvent.Timestamp,
                        MatchId = request.MatchId,
                        EventId = request.EventId
                    });
                }
                else
                {
                    _logger.LogError("Failed to create InfluxDB event {EventId} for match {MatchId}", 
                        request.EventId, request.MatchId);

                    return Result<CreateEventInfluxResponse>.Failure("Failed to store event in InfluxDB")
                        .WithCode(500);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating InfluxDB event {EventId} for match {MatchId}", 
                    request.EventId, request.MatchId);

                return Result<CreateEventInfluxResponse>.Failure($"An error occurred: {ex.Message}")
                    .WithCode(500);
            }
        }
    }
}