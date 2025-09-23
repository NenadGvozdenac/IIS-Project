using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Players.UpdatePlayer;

public class UpdatePlayerHandler : IRequestHandler<UpdatePlayerCommand, Result<UpdatePlayerResponse>>
{
    private readonly IPlayerRepository _playerRepository;

    public UpdatePlayerHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public Task<Result<UpdatePlayerResponse>> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingPlayer = _playerRepository.GetById(request.IdPlayer);
            
            if (existingPlayer == null)
            {
                return Task.FromResult(Result<UpdatePlayerResponse>.Failure($"Player with ID {request.IdPlayer} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.Name))
                existingPlayer.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Surname))
                existingPlayer.Surname = request.Surname;
            if (request.Birthday.HasValue)
                existingPlayer.Birthday = request.Birthday.Value;
            if (request.Weight.HasValue)
                existingPlayer.Weight = request.Weight.Value;
            if (request.Height.HasValue)
                existingPlayer.Height = request.Height.Value;
            if (request.IdNationality.HasValue)
                existingPlayer.IdNationality = request.IdNationality.Value;
            if (request.IdPosition.HasValue)
                existingPlayer.IdPosition = request.IdPosition.Value;

            var updatedPlayer = _playerRepository.Update(existingPlayer);

            var response = new UpdatePlayerResponse
            {
                IdPlayer = updatedPlayer.IdPlayer,
                Name = updatedPlayer.Name ?? string.Empty,
                Surname = updatedPlayer.Surname ?? string.Empty,
                Birthday = updatedPlayer.Birthday,
                Weight = updatedPlayer.Weight,
                Height = updatedPlayer.Height,
                IdNationality = updatedPlayer.IdNationality,
                IdPosition = updatedPlayer.IdPosition
            };

            return Task.FromResult(Result<UpdatePlayerResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdatePlayerResponse>.Failure($"An error occurred while updating player: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
