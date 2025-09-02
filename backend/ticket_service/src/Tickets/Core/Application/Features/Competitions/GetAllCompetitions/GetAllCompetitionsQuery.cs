using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Competitions.GetAllCompetitions;

public class GetAllCompetitionsQuery : IRequest<Result<IEnumerable<GetAllCompetitionsResponse>>>
{
}
