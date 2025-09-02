using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.Teams.DeleteTeam;

public record DeleteTeamCommand(int Id) : IRequest<Result<DeleteTeamResponse>>;
