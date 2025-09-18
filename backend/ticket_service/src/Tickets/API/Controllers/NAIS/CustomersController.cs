using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.GetAllCustomers;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.GetCustomerByEmail;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.GetCustomerById;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.CreateCustomer;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.UpdateCustomer;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.DeleteCustomer;
using ticket_service.src.Tickets.Core.Application.DTOs.Graph;

namespace ticket_service.src.Tickets.API.Controllers.NAIS;

[ApiController]
[Route("api/neo4j/[controller]")]
public class CustomersController : BaseController
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCustomers()
    {
        var query = new GetAllGraphCustomersQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> GetCustomerByEmail(string email)
    {
        var query = new GetGraphCustomerByEmailQuery(email);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetCustomerById(int id)
    {
        var query = new GetGraphCustomerByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto customerDto)
    {
        var command = new CreateGraphCustomerCommand(
            customerDto.Email,
            customerDto.Name,
            customerDto.Surname,
            customerDto.Phone,
            customerDto.Type);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("id/{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDto customerDto)
    {
        var command = new UpdateGraphCustomerCommand(
            id,
            customerDto.Email,
            customerDto.Name,
            customerDto.Surname,
            customerDto.Phone,
            customerDto.Type);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("id/{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var command = new DeleteGraphCustomerCommand(id);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}