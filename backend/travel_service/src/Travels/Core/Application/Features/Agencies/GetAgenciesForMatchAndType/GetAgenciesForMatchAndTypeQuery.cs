using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Agencies.GetAgenciesForMatchAndType;

public class GetAgenciesForMatchAndTypeQuery : IRequest<Result<GetAgenciesForMatchAndTypeResponse>>
{
    public int MatchId { get; set; }
    public string Type { get; set; } = string.Empty; // "transportation" or "accommodation"
}