namespace scouting_service.src.Elasticsearch.Models
{
    public class PlayerSearchRequest
    {
        public string SearchTerm { get; set; } = string.Empty;
        public string? Position { get; set; }
        public string? Nationality { get; set; }
        public int? MinHeight { get; set; }
        public int? MaxHeight { get; set; }
        public int? MinWeight { get; set; }
        public int? MaxWeight { get; set; }
        public int Size { get; set; } = 20;
        public int From { get; set; } = 0;
        public string SortBy { get; set; } = "name";
        public string SortOrder { get; set; } = "asc";
    }

    public class SessionSearchRequest
    {
        public string SearchTerm { get; set; } = string.Empty;
        public string? SessionType { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? TrainerName { get; set; }
        public int Size { get; set; } = 20;
        public int From { get; set; } = 0;
        public string SortBy { get; set; } = "startTime";
        public string SortOrder { get; set; } = "desc";
    }
}