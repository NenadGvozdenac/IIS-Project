using Cassandra.Mapping.Attributes;

namespace Travel_Service.src.Travels.Core.Domain.ColumnarEntities;

[Table("request", CaseSensitive = true)]
public class Request
{
    [PartitionKey]
    [Column("id_match")]
    public int IdMatch { get; set; }

    [ClusteringKey(0)]
    [Column("type")]
    public string Type { get; set; } = string.Empty;

    [ClusteringKey(1)]
    [Column("budget")]
    public int Budget { get; set; }

    [ClusteringKey(2)]
    [Column("id_request")]
    public int IdRequest { get; set; }

    [Column("state")]
    public string? State { get; set; }

    [Column("city")]
    public string? City { get; set; }

    [Column("hall")]
    public string? Hall { get; set; }
}