using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Matches.DeleteMatch;

public record DeleteMatchCommand(int Id, int UserId) : IRequest<Result<DeleteMatchResponse>>;
