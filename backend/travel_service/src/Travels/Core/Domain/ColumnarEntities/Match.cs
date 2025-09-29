using Cassandra.Mapping.Attributes;

namespace Travel_Service.src.Travels.Core.Domain.ColumnarEntities;

[Table("match", CaseSensitive = true)]
public class Match
{
    [PartitionKey(0)]
    [Column("city")]
    public string City { get; set; } = string.Empty;

    [PartitionKey(1)]
    [Column("type")]
    public string Type { get; set; } = string.Empty;

    [ClusteringKey(0)]
    [Column("scheduled_at")]
    public DateTimeOffset ScheduledAt { get; set; }

    [ClusteringKey(1)]
    [Column("id_match")]
    public int IdMatch { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("state")]
    public string? State { get; set; }

    [Column("hall")]
    public string? Hall { get; set; }

    [Column("is_in_our_hall")]
    public bool IsInOurHall { get; set; }

    [Column("transportation_required")]
    public bool TransportationRequired { get; set; }

    [Column("accommodation_required")]
    public bool AccommodationRequired { get; set; }
}