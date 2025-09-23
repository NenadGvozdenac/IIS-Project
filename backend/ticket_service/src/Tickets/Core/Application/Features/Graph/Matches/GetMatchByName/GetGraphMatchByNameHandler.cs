using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.GetMatchByName;

public class GetGraphMatchByNameHandler : IRequestHandler<GetGraphMatchByNameQuery, Result<GetGraphMatchByNameResponse>>
{
    private readonly IGraphMatchRepository _graphMatchRepository;

    public GetGraphMatchByNameHandler(IGraphMatchRepository graphMatchRepository)
    {
        _graphMatchRepository = graphMatchRepository;
    }

    public async Task<Result<GetGraphMatchByNameResponse>> Handle(GetGraphMatchByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var match = await _graphMatchRepository.GetMatchByName(request.Name);
            if (match == null)
            {
                return Result<GetGraphMatchByNameResponse>.Failure($"Match with name '{request.Name}' not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            var response = new GetGraphMatchByNameResponse
            {
                Name = match.Name,
                ScheduledAt = match.ScheduledAt,
                Type = match.Type,
                State = match.State,
                City = match.City,
                Hall = match.Hall,
                IsInOurHall = match.IsInOurHall
            };

            return Result<GetGraphMatchByNameResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetGraphMatchByNameResponse>.Failure($"An error occurred while retrieving the match: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}