using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.GetMatchById;

public class GetGraphMatchByIdHandler : IRequestHandler<GetGraphMatchByIdQuery, Result<GetGraphMatchByIdResponse>>
{
    private readonly IGraphMatchRepository _matchRepository;

    public GetGraphMatchByIdHandler(IGraphMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public async Task<Result<GetGraphMatchByIdResponse>> Handle(GetGraphMatchByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var match = await _matchRepository.GetMatchById(request.Id);
            if (match == null)
            {
                return Result<GetGraphMatchByIdResponse>.Failure("Match not found");
            }

            var response = new GetGraphMatchByIdResponse(match.Id, match.ElementId, match.Name, match.ScheduledAt, match.Type, match.State, match.City, match.Hall, match.IsInOurHall);
            return Result<GetGraphMatchByIdResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetGraphMatchByIdResponse>.Failure($"Failed to get match by id: {ex.Message}");
        }
    }
}