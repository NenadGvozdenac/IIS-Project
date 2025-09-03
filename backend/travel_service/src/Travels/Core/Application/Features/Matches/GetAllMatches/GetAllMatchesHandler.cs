using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Matches.GetAllMatches;

public class GetAllMatchesHandler : IRequestHandler<GetAllMatchesQuery, Result<GetAllMatchesResponse>>
{
    private readonly IMatchRepository _matchRepository;

    public GetAllMatchesHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public Task<Result<GetAllMatchesResponse>> Handle(GetAllMatchesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var matches = _matchRepository.GetAll();

            var response = new GetAllMatchesResponse
            {
                Matches = matches.Select(m => new MatchDto
                {
                    IdMatch = m.IdMatch,
                    Name = m.Name,
                    CreatedAt = m.CreatedAt,
                    Type = m.Type,
                    State = m.State,
                    City = m.City,
                    Hall = m.Hall,
                    IsInOurHall = m.IsInOurHall,
                    TransportationRequired = m.TransportationRequired,
                    AccommodationRequired = m.AccommodationRequired,
                    IdCompetition = m.IdCompetition,
                    IdSeason = m.IdSeason,
                    IdTeam = m.IdTeam
                }).ToList()
            };

            return Task.FromResult(Result<GetAllMatchesResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetAllMatchesResponse>.Failure($"An error occurred while retrieving matches: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
