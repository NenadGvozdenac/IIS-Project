using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.Nationality.GetAllNationality;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NationalityController : BaseController
{
    private readonly IMediator _mediator;

    public NationalityController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    public async Task<ActionResult> GetAllNationalities()
    {
        var query = new GetAllNationalityQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
}
