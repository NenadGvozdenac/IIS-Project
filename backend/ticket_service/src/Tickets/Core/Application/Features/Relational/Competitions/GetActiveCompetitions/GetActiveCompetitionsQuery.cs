using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Competitions.GetActiveCompetitions;

public class GetActiveCompetitionsQuery : IRequest<Result<IEnumerable<GetActiveCompetitionsResponse>>>
{
}
