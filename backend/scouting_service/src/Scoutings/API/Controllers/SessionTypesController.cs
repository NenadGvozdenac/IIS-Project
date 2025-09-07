using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionTypesController : BaseController
{
    private readonly ISessionTypeRepository _sessionTypeRepository;

    public SessionTypesController(ISessionTypeRepository sessionTypeRepository)
    {
        _sessionTypeRepository = sessionTypeRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<SessionType>> GetAll()
    {
        try
        {
            var sessionTypes = _sessionTypeRepository.GetAll();
            var result = Result<IEnumerable<SessionType>>.Success(sessionTypes);
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving session types: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<SessionType> GetById(int id)
    {
        try
        {
            var sessionType = _sessionTypeRepository.GetById(id);
            if (sessionType == null)
            {
                var result = Result.Failure($"Session type with ID {id} not found").WithCode((int)ResultCode.NotFound);
                return CreateResponse(result);
            }

            var successResult = Result<SessionType>.Success(sessionType);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving session type: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpPost]
    public ActionResult<SessionType> Create([FromBody] CreateSessionTypeRequest request)
    {
        try
        {
            if (request == null)
            {
                var result = Result.Failure("Session type data is required").WithCode((int)ResultCode.BadRequest);
                return CreateResponse(result);
            }

            var sessionType = new SessionType
            {
                Type = request.Type
            };

            var createdSessionType = _sessionTypeRepository.Create(sessionType);
            var successResult = Result<SessionType>.Success(createdSessionType);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error creating session type: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }
}

// Request DTOs
public class CreateSessionTypeRequest
{
    public string? Type { get; set; }
}
