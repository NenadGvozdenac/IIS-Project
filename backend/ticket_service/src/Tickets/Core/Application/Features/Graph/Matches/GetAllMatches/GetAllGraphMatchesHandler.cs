using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.GetAllMatches;

public class GetAllGraphMatchesHandler : IRequestHandler<GetAllGraphMatchesQuery, Result<List<GetAllGraphMatchesResponse>>>
{
    private readonly IGraphMatchRepository _graphMatchRepository;

    public GetAllGraphMatchesHandler(IGraphMatchRepository graphMatchRepository)
    {
        _graphMatchRepository = graphMatchRepository;
    }

    public async Task<Result<List<GetAllGraphMatchesResponse>>> Handle(GetAllGraphMatchesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var matches = await _graphMatchRepository.GetAllMatches();
            var response = matches.Select(match => new GetAllGraphMatchesResponse
            {
                Id = match.Id,
                ElementId = match.ElementId,
                Name = match.Name,
                ScheduledAt = match.ScheduledAt,
                Type = match.Type,
                State = match.State,
                City = match.City,
                Hall = match.Hall,
                IsInOurHall = match.IsInOurHall
            }).ToList();

            return Result<List<GetAllGraphMatchesResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<List<GetAllGraphMatchesResponse>>.Failure($"An error occurred while retrieving matches: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}