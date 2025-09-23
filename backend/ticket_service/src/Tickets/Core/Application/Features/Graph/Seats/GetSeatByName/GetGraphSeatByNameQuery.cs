using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetSeatByName;

public class GetGraphSeatByNameQuery : IRequest<Result<GetGraphSeatByNameResponse>>
{
    public string Name { get; set; } = string.Empty;

    public GetGraphSeatByNameQuery(string name)
    {
        Name = name;
    }
}