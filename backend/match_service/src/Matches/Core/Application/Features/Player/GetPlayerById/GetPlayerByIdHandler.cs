using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.Player.GetPlayerById;

public class GetPlayerByIdHandler : IRequestHandler<GetPlayerByIdQuery, Result<GetPlayerByIdResponse>>
{
    private readonly IPlayerRepository _playerRepository;

    public GetPlayerByIdHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public Task<Result<GetPlayerByIdResponse>> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var player = _playerRepository.GetById(request.Id);

            if (player == null)
            {
                return Task.FromResult(Result<GetPlayerByIdResponse>.Failure($"Player with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetPlayerByIdResponse
            {
                IdPlayer = player.IdPlayer,
                Name = player.Name,
                Surname = player.Surname,
                Birthday = player.Birthday,
                Weight = player.Weight,
                Height = player.Height,
                IdNationality = player.IdNationality,
                NationalityState = player.IdNationalityNavigation?.State,
                IdPosition = player.IdPosition,
                PositionName = player.IdPositionNavigation?.Name
            };

            return Task.FromResult(Result<GetPlayerByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetPlayerByIdResponse>.Failure($"An error occurred while retrieving the player: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
