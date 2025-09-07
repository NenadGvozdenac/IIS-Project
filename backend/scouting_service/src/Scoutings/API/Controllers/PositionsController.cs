using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController : BaseController
{
    private readonly IPositionRepository _positionRepository;

    public PositionsController(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Position>> GetAll()
    {
        try
        {
            var positions = _positionRepository.GetAll();
            var result = Result<IEnumerable<Position>>.Success(positions);
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving positions: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Position> GetById(int id)
    {
        try
        {
            var position = _positionRepository.GetById(id);
            if (position == null)
            {
                var result = Result.Failure($"Position with ID {id} not found").WithCode((int)ResultCode.NotFound);
                return CreateResponse(result);
            }

            var successResult = Result<Position>.Success(position);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving position: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpPost]
    public ActionResult<Position> Create([FromBody] CreatePositionRequest request)
    {
        try
        {
            if (request == null)
            {
                var result = Result.Failure("Position data is required").WithCode((int)ResultCode.BadRequest);
                return CreateResponse(result);
            }

            var position = new Position
            {
                Name = request.Name
            };

            var createdPosition = _positionRepository.Create(position);
            var successResult = Result<Position>.Success(createdPosition);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error creating position: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }
}

// Request DTOs
public class CreatePositionRequest
{
    public string Name { get; set; } = null!;
}
