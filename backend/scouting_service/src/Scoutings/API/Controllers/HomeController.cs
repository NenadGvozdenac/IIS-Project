using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/home")]
public class HomeController : BaseController
{
    [HttpGet]
    public IActionResult Get()
    {
        return CreateResponse(Result<object>.Success(new
        {
            message = "Welcome to Scouting Service API"
        }));
    }
}
