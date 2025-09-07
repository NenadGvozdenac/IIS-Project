using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : BaseController
{
    private readonly ISessionRepository _sessionRepository;

    public SessionsController(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Session>> GetAll()
    {
        try
        {
            var sessions = _sessionRepository.GetAll();
            var result = Result<IEnumerable<Session>>.Success(sessions);
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving sessions: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Session> GetById(int id)
    {
        try
        {
            var session = _sessionRepository.GetById(id);
            if (session == null)
            {
                var result = Result.Failure($"Session with ID {id} not found").WithCode((int)ResultCode.NotFound);
                return CreateResponse(result);
            }

            var successResult = Result<Session>.Success(session);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving session: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpPost]
    public ActionResult<Session> Create([FromBody] CreateSessionRequest request)
    {
        try
        {
            if (request == null)
            {
                var result = Result.Failure("Session data is required").WithCode((int)ResultCode.BadRequest);
                return CreateResponse(result);
            }

            var session = new Session
            {
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IdSessionStatus = request.IdSessionStatus,
                IdSessionType = request.IdSessionType,
                IdUser = request.IdUser,
                IdPlayer = request.IdPlayer,
                Note = request.Note
            };

            var createdSession = _sessionRepository.Create(session);
            var successResult = Result<Session>.Success(createdSession);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error creating session: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }
}

// Request DTOs
public class CreateSessionRequest
{
    public DateOnly? StartTime { get; set; }
    public DateOnly? EndTime { get; set; }
    public int IdSessionStatus { get; set; }
    public int IdSessionType { get; set; }
    public int IdUser { get; set; }
    public int IdPlayer { get; set; }
    public string? Note { get; set; }
}
