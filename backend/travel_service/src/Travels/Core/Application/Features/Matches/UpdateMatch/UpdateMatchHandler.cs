using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Features.Matches.UpdateMatch;

public class UpdateMatchHandler : IRequestHandler<UpdateMatchCommand, Result<UpdateMatchResponse>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;

    public UpdateMatchHandler(IMatchRepository matchRepository, IUserRepository userRepository)
    {
        _matchRepository = matchRepository;
        _userRepository = userRepository;
    }

    public Task<Result<UpdateMatchResponse>> Handle(UpdateMatchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingMatch = _matchRepository.GetById(request.Id);
            if (existingMatch == null)
            {
                return Task.FromResult(Result<UpdateMatchResponse>.Failure($"Match with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            if (existingMatch.CreatedAt <= DateTime.Now)
            {
                return Task.FromResult(Result<UpdateMatchResponse>.Failure("Cannot update match that has already started or finished")
                    .WithCode((int)ResultCode.Forbidden));
            }

            var user = _userRepository.GetById(request.UserId);
            if (user == null)
            {
                return Task.FromResult(Result<UpdateMatchResponse>.Failure("User not found")
                    .WithCode((int)ResultCode.NotFound));
            }
            if (user.Type != "team manager")
            {
                return Task.FromResult(Result<UpdateMatchResponse>.Failure("Only team manager can update matches")
                    .WithCode((int)ResultCode.Forbidden));
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Task.FromResult(Result<UpdateMatchResponse>.Failure("Match name is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (string.IsNullOrWhiteSpace(request.Type))
            {
                return Task.FromResult(Result<UpdateMatchResponse>.Failure("Match type is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (string.IsNullOrWhiteSpace(request.State))
            {
                return Task.FromResult(Result<UpdateMatchResponse>.Failure("State is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (string.IsNullOrWhiteSpace(request.City))
            {
                return Task.FromResult(Result<UpdateMatchResponse>.Failure("City is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (string.IsNullOrWhiteSpace(request.Hall))
            {
                return Task.FromResult(Result<UpdateMatchResponse>.Failure("Hall is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (request.IdSeason == 0)
            {
                return Task.FromResult(Result<UpdateMatchResponse>.Failure("SeasonId is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (request.IdTeam == 0)
            {
                return Task.FromResult(Result<UpdateMatchResponse>.Failure("TeamId is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            existingMatch.Name = request.Name;
            existingMatch.CreatedAt = request.CreatedAt;
            existingMatch.Type = request.Type;
            existingMatch.State = request.State;
            existingMatch.City = request.City;
            existingMatch.Hall = request.Hall;
            existingMatch.IsInOurHall = request.IsInOurHall;
            existingMatch.TransportationRequired = request.TransportationRequired;
            existingMatch.AccommodationRequired = request.AccommodationRequired;
            existingMatch.IdCompetition = request.IdCompetition;
            existingMatch.IdSeason = request.IdSeason;
            existingMatch.IdTeam = request.IdTeam;

            var updatedMatch = _matchRepository.Update(existingMatch);

            var response = new UpdateMatchResponse
            {
                IdMatch = updatedMatch.IdMatch,
                Name = updatedMatch.Name,
                CreatedAt = updatedMatch.CreatedAt,
                Type = updatedMatch.Type,
                State = updatedMatch.State,
                City = updatedMatch.City,
                Hall = updatedMatch.Hall,
                IsInOurHall = updatedMatch.IsInOurHall,
                TransportationRequired = updatedMatch.TransportationRequired,
                AccommodationRequired = updatedMatch.AccommodationRequired,
                IdCompetition = updatedMatch.IdCompetition,
                IdSeason = updatedMatch.IdSeason,
                IdTeam = updatedMatch.IdTeam
            };

            return Task.FromResult(Result<UpdateMatchResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateMatchResponse>.Failure($"An error occurred while updating the match: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}