using Cassandra.Mapping.Attributes;

namespace Travel_Service.src.Travels.Core.Domain.ColumnarEntities;

[Table("offer", CaseSensitive = true)]
public class Offer
{
    [PartitionKey]
    [Column("id_match")]
    public int IdMatch { get; set; }

    [ClusteringKey(0)]
    [Column("type")]
    public string? Type { get; set; }

    [ClusteringKey(1)]
    [Column("chosen")]
    public bool? Chosen { get; set; }

    [ClusteringKey(2)]
    [Column("price")]
    public int Price { get; set; }

    [ClusteringKey(3)]
    [Column("id_offer")]
    public int IdOffer { get; set; }

    [Column("user_id_user")]
    public int? UserIdUser { get; set; }

    [Column("id_request")]
    public int IdRequest { get; set; }

    [Column("score")]
    public decimal? Score { get; set; }
}