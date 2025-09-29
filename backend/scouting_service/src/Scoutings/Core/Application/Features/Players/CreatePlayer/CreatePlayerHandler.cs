using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Features.Players.CreatePlayer;

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
            var player = new Player
            {
                Name = request.Name ?? string.Empty,
                Surname = request.Surname ?? string.Empty,
                Birthday = request.Birthday,
                Weight = request.Weight,
                Height = request.Height,
                IdNationality = request.IdNationality,
                IdPosition = request.IdPosition
            };

            var createdPlayer = _playerRepository.Create(player);

            var response = new CreatePlayerResponse
            {
                IdPlayer = createdPlayer.IdPlayer,
                Name = createdPlayer.Name ?? string.Empty,
                Surname = createdPlayer.Surname ?? string.Empty,
                Birthday = createdPlayer.Birthday,
                Weight = createdPlayer.Weight,
                Height = createdPlayer.Height,
                IdNationality = createdPlayer.IdNationality,
                IdPosition = createdPlayer.IdPosition
            };

            return Task.FromResult(Result<CreatePlayerResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreatePlayerResponse>.Failure($"An error occurred while creating player: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
