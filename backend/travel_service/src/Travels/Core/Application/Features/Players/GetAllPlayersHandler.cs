using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Players.GetAllPlayers;

public class GetAllPlayersHandler : IRequestHandler<GetAllPlayersQuery, Result<List<GetAllPlayersResponse>>>
{
    private readonly IPlayerRepository _playerRepository;

    public GetAllPlayersHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public Task<Result<List<GetAllPlayersResponse>>> Handle(GetAllPlayersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var players = _playerRepository.GetAll();
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var response = players.Select(p => new GetAllPlayersResponse
            {
                IdPlayer = p.IdPlayer,
                Name = p.Name,
                Surname = p.Surname,
                Birthday = p.Birthday,
                IdNationality = p.IdNationality,
                IdPosition = p.IdPosition
            }).ToList();

            return Task.FromResult(Result<List<GetAllPlayersResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllPlayersResponse>>.Failure($"An error occurred while retrieving players: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
