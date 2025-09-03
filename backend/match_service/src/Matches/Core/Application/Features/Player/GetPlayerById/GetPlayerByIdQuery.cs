using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.Player.GetPlayerById;

public record GetPlayerByIdQuery(int Id) : IRequest<Result<GetPlayerByIdResponse>>;
