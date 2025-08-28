using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/home")]
public class HomeController : BaseController
{
    [HttpGet]
    public IActionResult Get()
    {
        return CreateResponse(Result<object>.Success(new
        {
            message = "Welcome to Travel Service API"
        }));
    }
}
