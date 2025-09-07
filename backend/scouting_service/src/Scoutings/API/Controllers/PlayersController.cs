using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : BaseController
{
    private readonly IPlayerRepository _playerRepository;

    public PlayersController(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Player>> GetAll()
    {
        try
        {
            var players = _playerRepository.GetAll();
            var result = Result<IEnumerable<Player>>.Success(players);
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving players: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Player> GetById(int id)
    {
        try
        {
            var player = _playerRepository.GetById(id);
            if (player == null)
            {
                var result = Result.Failure($"Player with ID {id} not found").WithCode((int)ResultCode.NotFound);
                return CreateResponse(result);
            }

            var successResult = Result<Player>.Success(player);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving player: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpPost]
    public ActionResult<Player> Create([FromBody] CreatePlayerRequest request)
    {
        try
        {
            if (request == null)
            {
                var result = Result.Failure("Player data is required").WithCode((int)ResultCode.BadRequest);
                return CreateResponse(result);
            }

            var player = new Player
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
            var successResult = Result<Player>.Success(createdPlayer);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error creating player: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }
}

// Request DTOs
public class CreatePlayerRequest
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public DateOnly? Birthday { get; set; }
    public int? Weight { get; set; }
    public int? Height { get; set; }
    public int IdNationality { get; set; }
    public int IdPosition { get; set; }
}
