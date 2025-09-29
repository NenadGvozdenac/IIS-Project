using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetSeasonPlayerAverages
{
    public class GetSeasonPlayerAveragesQuery : IRequest<Result<GetSeasonPlayerAveragesResponse>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? TeamId { get; set; }
        public int MinMatches { get; set; } = 1;
    }
}