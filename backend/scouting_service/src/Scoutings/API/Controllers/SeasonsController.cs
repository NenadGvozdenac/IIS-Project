using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeasonsController : BaseController
{
    private readonly ISeasonRepository _seasonRepository;

    public SeasonsController(ISeasonRepository seasonRepository)
    {
        _seasonRepository = seasonRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Season>> GetAll()
    {
        try
        {
            var seasons = _seasonRepository.GetAll();
            var result = Result<IEnumerable<Season>>.Success(seasons);
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving seasons: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Season> GetById(int id)
    {
        try
        {
            var season = _seasonRepository.GetById(id);
            if (season == null)
            {
                var result = Result.Failure($"Season with ID {id} not found").WithCode((int)ResultCode.NotFound);
                return CreateResponse(result);
            }

            var successResult = Result<Season>.Success(season);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving season: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpPost]
    public ActionResult<Season> Create([FromBody] CreateSeasonRequest request)
    {
        try
        {
            if (request == null)
            {
                var result = Result.Failure("Season data is required").WithCode((int)ResultCode.BadRequest);
                return CreateResponse(result);
            }

            var season = new Season
            {
                StartedAt = request.StartedAt,
                EndedAt = request.EndedAt,
                Name = request.Name
            };

            var createdSeason = _seasonRepository.Create(season);
            var successResult = Result<Season>.Success(createdSeason);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error creating season: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }
}

// Request DTOs
public class CreateSeasonRequest
{
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public string Name { get; set; } = null!;
}
