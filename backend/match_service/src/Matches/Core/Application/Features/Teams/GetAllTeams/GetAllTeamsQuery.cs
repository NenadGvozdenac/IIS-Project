using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.Teams.GetAllTeams;

public record GetAllTeamsQuery() : IRequest<Result<GetAllTeamsResponse>>;
