using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.CreateMatch;

public class CreateGraphMatchHandler : IRequestHandler<CreateGraphMatchCommand, Result<CreateGraphMatchResponse>>
{
    private readonly IGraphMatchRepository _graphMatchRepository;

    public CreateGraphMatchHandler(IGraphMatchRepository graphMatchRepository)
    {
        _graphMatchRepository = graphMatchRepository;
    }

    public async Task<Result<CreateGraphMatchResponse>> Handle(CreateGraphMatchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result<CreateGraphMatchResponse>.Failure("Match name is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(request.Type))
            {
                return Result<CreateGraphMatchResponse>.Failure("Match type is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            // Check if match already exists
            var existingMatch = await _graphMatchRepository.GetMatchByName(request.Name);
            if (existingMatch != null)
            {
                return Result<CreateGraphMatchResponse>.Failure($"Match with name '{request.Name}' already exists")
                    .WithCode((int)ResultCode.Conflict);
            }

            var match = new Match
            {
                Name = request.Name,
                ScheduledAt = request.ScheduledAt,
                Type = request.Type,
                State = request.State,
                City = request.City,
                Hall = request.Hall,
                IsInOurHall = request.IsInOurHall
            };

            await _graphMatchRepository.CreateMatch(match);

            var response = new CreateGraphMatchResponse
            {
                Name = match.Name,
                ScheduledAt = match.ScheduledAt,
                Type = match.Type,
                State = match.State,
                City = match.City,
                Hall = match.Hall,
                IsInOurHall = match.IsInOurHall
            };

            return Result<CreateGraphMatchResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CreateGraphMatchResponse>.Failure($"An error occurred while creating the match: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}