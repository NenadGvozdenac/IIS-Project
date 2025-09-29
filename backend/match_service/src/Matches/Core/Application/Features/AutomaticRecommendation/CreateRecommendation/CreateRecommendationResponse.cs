namespace match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.CreateRecommendation
{
    public class CreateRecommendationResponse
    {
        public int IdRecommendation { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; }
        public string Period { get; set; } = string.Empty;
        public int PeriodTime { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int IdMatch { get; set; }
    }
}