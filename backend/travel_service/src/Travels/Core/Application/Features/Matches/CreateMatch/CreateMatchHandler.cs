using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Features.Matches.CreateMatch;

public class CreateMatchHandler : IRequestHandler<CreateMatchCommand, Result<CreateMatchResponse>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;

    public CreateMatchHandler(IMatchRepository matchRepository, IUserRepository userRepository)
    {
        _matchRepository = matchRepository;
        _userRepository = userRepository;
    }

    public Task<Result<CreateMatchResponse>> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = _userRepository.GetById(request.UserId);
            if (user == null)
            {
                return Task.FromResult(Result<CreateMatchResponse>.Failure("User not found")
                    .WithCode((int)ResultCode.NotFound));
            }
            if (user.Type != "team manager")
            {
                return Task.FromResult(Result<CreateMatchResponse>.Failure("Only team manager can create matches")
                    .WithCode((int)ResultCode.Forbidden));
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Task.FromResult(Result<CreateMatchResponse>.Failure("Match name is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.Type))
            {
                return Task.FromResult(Result<CreateMatchResponse>.Failure("Match type is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.State))
            {
                return Task.FromResult(Result<CreateMatchResponse>.Failure("State is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.City))
            {
                return Task.FromResult(Result<CreateMatchResponse>.Failure("City is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.Hall))
            {
                return Task.FromResult(Result<CreateMatchResponse>.Failure("Hall is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (request.SeasonId == 0)
            {
                return Task.FromResult(Result<CreateMatchResponse>.Failure("SeasonId is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (request.TeamId == 0)
            {
                return Task.FromResult(Result<CreateMatchResponse>.Failure("TeamId is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            var match = new Match
            {
                Name = request.Name,
                ScheduledAt = request.ScheduledAt,
                Type = request.Type,
                State = request.State,
                City = request.City,
                Hall = request.Hall,
                IsInOurHall = request.IsInOurHall,
                TransportationRequired = request.TransportationRequired,
                AccommodationRequired = request.AccommodationRequired,
                IdSeason = request.SeasonId,
                IdTeam = request.TeamId,
                IdCompetition = request.CompetitionId
            };

            var createdMatch = _matchRepository.Create(match);

            var response = new CreateMatchResponse
            {
                IdMatch = createdMatch.IdMatch,
                Name = createdMatch.Name,
                ScheduledAt = createdMatch.ScheduledAt,
                Type = createdMatch.Type,
                State = createdMatch.State,
                City = createdMatch.City,
                Hall = createdMatch.Hall,
                IsInOurHall = createdMatch.IsInOurHall,
                TransportationRequired = createdMatch.TransportationRequired,
                AccommodationRequired = createdMatch.AccommodationRequired,
                IdSeason = createdMatch.IdSeason,
                IdTeam = createdMatch.IdTeam,
                IdCompetition = createdMatch.IdCompetition
            };

            return Task.FromResult(Result<CreateMatchResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateMatchResponse>.Failure($"An error occurred while creating the match: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}