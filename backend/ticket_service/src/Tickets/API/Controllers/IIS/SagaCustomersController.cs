using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.CreateCustomer;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.DeleteCustomer;
using ticket_service.src.Tickets.Core.Application.DTOs.Graph;

namespace ticket_service.src.Tickets.API.Controllers.IIS;

[ApiController]
[Route("api/saga/[controller]")]
public class CustomersController : BaseController
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a customer as part of SAGA orchestration
    /// This endpoint is specifically designed for SAGA transactions from the auth service
    /// </summary>
    /// <param name="customerDto">Customer data from auth service</param>
    /// <returns>Customer creation result</returns>
    [HttpPost]
    public async Task<IActionResult> CreateCustomerForSaga([FromBody] CreateCustomerDto customerDto)
    {
        try
        {
            var command = new CreateGraphCustomerCommand(
                customerDto.Email,
                customerDto.Name,
                customerDto.Surname,
                customerDto.Phone,
                customerDto.Type);
            
            var result = await _mediator.Send(command);
            
            // Return standardized response for SAGA orchestration
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "Customer created successfully",
                    data = result.Value
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Error,
                    data = default(object)
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Internal server error: {ex.Message}",
                data = default(object)
            });
        }
    }

    /// <summary>
    /// Deletes a customer as part of SAGA compensation
    /// This endpoint is used for rollback operations in SAGA transactions
    /// </summary>
    /// <param name="id">Customer ID to delete</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomerForSaga(int id)
    {
        try
        {
            var command = new DeleteGraphCustomerCommand(id);
            var result = await _mediator.Send(command);
            
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "Customer deleted successfully"
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Error
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Internal server error: {ex.Message}"
            });
        }
    }
}