using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.UpdateMatch;

public class UpdateGraphMatchHandler : IRequestHandler<UpdateGraphMatchCommand, Result<UpdateGraphMatchResponse>>
{
    private readonly IGraphMatchRepository _graphMatchRepository;

    public UpdateGraphMatchHandler(IGraphMatchRepository graphMatchRepository)
    {
        _graphMatchRepository = graphMatchRepository;
    }

    public async Task<Result<UpdateGraphMatchResponse>> Handle(UpdateGraphMatchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Type))
            {
                return Result<UpdateGraphMatchResponse>.Failure("Match type is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            // Check if match exists
            var existingMatch = await _graphMatchRepository.GetMatchByName(request.Name);
            if (existingMatch == null)
            {
                return Result<UpdateGraphMatchResponse>.Failure($"Match with name '{request.Name}' not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            var updatedMatch = new Match
            {
                Name = string.IsNullOrWhiteSpace(request.NewName) ? request.Name : request.NewName,
                ScheduledAt = request.ScheduledAt,
                Type = request.Type,
                State = request.State,
                City = request.City,
                Hall = request.Hall,
                IsInOurHall = request.IsInOurHall
            };

            await _graphMatchRepository.UpdateMatch(updatedMatch);

            var response = new UpdateGraphMatchResponse
            {
                Name = updatedMatch.Name,
                ScheduledAt = updatedMatch.ScheduledAt,
                Type = updatedMatch.Type,
                State = updatedMatch.State,
                City = updatedMatch.City,
                Hall = updatedMatch.Hall,
                IsInOurHall = updatedMatch.IsInOurHall
            };

            return Result<UpdateGraphMatchResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<UpdateGraphMatchResponse>.Failure($"An error occurred while updating the match: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}