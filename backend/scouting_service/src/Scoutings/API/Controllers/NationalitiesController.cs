using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NationalitiesController : BaseController
{
    private readonly INationalityRepository _nationalityRepository;

    public NationalitiesController(INationalityRepository nationalityRepository)
    {
        _nationalityRepository = nationalityRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Nationality>> GetAll()
    {
        try
        {
            var nationalities = _nationalityRepository.GetAll();
            var result = Result<IEnumerable<Nationality>>.Success(nationalities);
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving nationalities: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Nationality> GetById(int id)
    {
        try
        {
            var nationality = _nationalityRepository.GetById(id);
            if (nationality == null)
            {
                var result = Result.Failure($"Nationality with ID {id} not found").WithCode((int)ResultCode.NotFound);
                return CreateResponse(result);
            }

            var successResult = Result<Nationality>.Success(nationality);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving nationality: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpPost]
    public ActionResult<Nationality> Create([FromBody] CreateNationalityRequest request)
    {
        try
        {
            if (request == null)
            {
                var result = Result.Failure("Nationality data is required").WithCode((int)ResultCode.BadRequest);
                return CreateResponse(result);
            }

            var nationality = new Nationality
            {
                State = request.State
            };

            var createdNationality = _nationalityRepository.Create(nationality);
            var successResult = Result<Nationality>.Success(createdNationality);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error creating nationality: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }
}

// Request DTOs
public class CreateNationalityRequest
{
    public string? State { get; set; }
}
