using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.Teams.GetTeamById;

public record GetTeamByIdQuery(int Id) : IRequest<Result<GetTeamByIdResponse>>;
