using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Features.Player.CreatePlayer;

public class CreatePlayerHandler : IRequestHandler<CreatePlayerCommand, Result<CreatePlayerResponse>>
{
    private readonly IPlayerRepository _playerRepository;

    public CreatePlayerHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public Task<Result<CreatePlayerResponse>> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Task.FromResult(Result<CreatePlayerResponse>.Failure("Player name is required.")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.Surname))
            {
                return Task.FromResult(Result<CreatePlayerResponse>.Failure("Player surname is required.")
                    .WithCode((int)ResultCode.BadRequest));
            }

            var player = new Domain.Entities.Player
            {
                Name = request.Name,
                Surname = request.Surname,
                Birthday = request.Birthday,
                Weight = request.Weight,
                Height = request.Height,
                IdNationality = request.IdNationality,
                IdPosition = request.IdPosition
            };

            var createdPlayer = _playerRepository.Create(player);

            // Get the player with navigation properties for response
            var playerWithDetails = _playerRepository.GetById(createdPlayer.IdPlayer);

            var response = new CreatePlayerResponse
            {
                IdPlayer = playerWithDetails.IdPlayer,
                Name = playerWithDetails.Name,
                Surname = playerWithDetails.Surname,
                Birthday = playerWithDetails.Birthday,
                Weight = playerWithDetails.Weight,
                Height = playerWithDetails.Height,
                IdNationality = playerWithDetails.IdNationality,
                NationalityState = playerWithDetails.IdNationalityNavigation?.State,
                IdPosition = playerWithDetails.IdPosition,
                PositionName = playerWithDetails.IdPositionNavigation?.Name
            };

            return Task.FromResult(Result<CreatePlayerResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreatePlayerResponse>.Failure($"An error occurred while creating the player: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
