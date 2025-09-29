namespace match_service.src.Matches.Core.Application.Features.MatchTracking.PrepareMatchTracking
{
    public class PrepareMatchTrackingResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int MatchId { get; set; }
        public string? NewTrackingStatus { get; set; }
    }
}
