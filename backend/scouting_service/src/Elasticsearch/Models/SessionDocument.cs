using Nest;

namespace scouting_service.src.Elasticsearch.Models
{
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class SessionDocument
    {
        public int Id { get; set; }

        [Date(Format = "yyyy-MM-dd'T'HH:mm:ss||yyyy-MM-dd HH:mm:ss")]
        public DateTime StartTime { get; set; }

        [Date(Format = "yyyy-MM-dd'T'HH:mm:ss||yyyy-MM-dd HH:mm:ss")]
        public DateTime EndTime { get; set; }

        public int DurationMinutes { get; set; }

        [Object]
        public SessionStatusInfo SessionStatus { get; set; } = new();

        [Object]
        public SessionTypeInfo SessionType { get; set; } = new();

        [Object]
        public UserInfo User { get; set; } = new();

        [Nested]
        public List<SessionParticipant> Participants { get; set; } = new();

        [Text(Analyzer = "standard")]
        public string Notes { get; set; } = string.Empty;

        [Text(Analyzer = "standard")]
        public string Description { get; set; } = string.Empty;

        [Keyword]
        public List<string> Tags { get; set; } = new();

        [Text(Analyzer = "standard")]
        public string Location { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class SessionStatusInfo
    {
        public int Id { get; set; }

        [Text(Analyzer = "standard")]
        [Keyword(Name = "status.keyword")]
        public string Status { get; set; } = string.Empty;
    }

    public class SessionTypeInfo
    {
        public int Id { get; set; }

        [Text(Analyzer = "standard")]
        [Keyword(Name = "type.keyword")]
        public string Type { get; set; } = string.Empty;
    }

    public class UserInfo
    {
        public int Id { get; set; }

        [Text(Analyzer = "standard")]
        [Keyword(Name = "name.keyword")]
        public string Name { get; set; } = string.Empty;

        [Text(Analyzer = "standard")]
        [Keyword(Name = "surname.keyword")]
        public string Surname { get; set; } = string.Empty;

        [Keyword]
        public string Email { get; set; } = string.Empty;
    }

    public class SessionParticipant
    {
        public int PlayerId { get; set; }

        [Text(Analyzer = "standard")]
        [Keyword(Name = "player_name.keyword")]
        public string PlayerName { get; set; } = string.Empty;

        [Text(Analyzer = "standard")]
        [Keyword(Name = "player_surname.keyword")]
        public string PlayerSurname { get; set; } = string.Empty;

        [Text(Analyzer = "standard")]
        [Keyword(Name = "position.keyword")]
        public string Position { get; set; } = string.Empty;
    }
}