namespace match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.AcceptRecommendation
{
    public class AcceptRecommendationResponse
    {
        public int IdRecommendation { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}