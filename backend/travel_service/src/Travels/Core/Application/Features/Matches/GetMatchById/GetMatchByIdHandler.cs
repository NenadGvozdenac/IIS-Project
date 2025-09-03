using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Matches.GetMatchById;

public class GetMatchByIdHandler : IRequestHandler<GetMatchByIdQuery, Result<GetMatchByIdResponse>>
{
    private readonly IMatchRepository _matchRepository;

    public GetMatchByIdHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public Task<Result<GetMatchByIdResponse>> Handle(GetMatchByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var match = _matchRepository.GetById(request.Id);

            if (match == null)
            {
                return Task.FromResult(Result<GetMatchByIdResponse>.Failure($"Match with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetMatchByIdResponse
            {
                IdMatch = match.IdMatch,
                Name = match.Name,
                CreatedAt = match.CreatedAt,
                Type = match.Type,
                State = match.State,
                City = match.City,
                Hall = match.Hall,
                IsInOurHall = match.IsInOurHall,
                TransportationRequired = match.TransportationRequired,
                AccommodationRequired = match.AccommodationRequired,
                IdCompetition = match.IdCompetition,
                IdSeason = match.IdSeason,
                IdTeam = match.IdTeam
            };

            return Task.FromResult(Result<GetMatchByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetMatchByIdResponse>.Failure($"An error occurred while retrieving the match: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
