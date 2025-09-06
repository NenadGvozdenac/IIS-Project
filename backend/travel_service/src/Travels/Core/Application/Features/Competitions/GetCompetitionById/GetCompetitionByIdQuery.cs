using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Competitions.GetCompetitionById;

public record GetCompetitionByIdQuery(int Id) : IRequest<Result<GetCompetitionByIdResponse>>;
