using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.GetAllIndividualTickets;

public class GetAllGraphIndividualTicketsHandler : IRequestHandler<GetAllGraphIndividualTicketsQuery, Result<List<GetAllGraphIndividualTicketsResponse>>>
{
    private readonly IGraphIndividualTicketRepository _graphIndividualTicketRepository;

    public GetAllGraphIndividualTicketsHandler(IGraphIndividualTicketRepository graphIndividualTicketRepository)
    {
        _graphIndividualTicketRepository = graphIndividualTicketRepository;
    }

    public async Task<Result<List<GetAllGraphIndividualTicketsResponse>>> Handle(GetAllGraphIndividualTicketsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var tickets = await _graphIndividualTicketRepository.GetAllIndividualTickets();
            var response = tickets.Select(ticket => new GetAllGraphIndividualTicketsResponse
            {
                Id = ticket.Id,
                ElementId = ticket.ElementId,
                Name = ticket.Name,
                Description = ticket.Description,
                Type = ticket.Type,
                ReleasedAt = ticket.ReleasedAt,
                Price = ticket.Price
            }).ToList();

            return Result<List<GetAllGraphIndividualTicketsResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<List<GetAllGraphIndividualTicketsResponse>>.Failure($"An error occurred while retrieving individual tickets: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}