using Cassandra.Mapping.Attributes;

namespace Travel_Service.src.Travels.Core.Domain.ColumnarEntities;

[Table("trip", CaseSensitive = true)]
public class Trip
{
    [PartitionKey]
    [Column("match_id_match")]
    public int MatchIdMatch { get; set; }

    [ClusteringKey]
    [Column("id_trip")]
    public int IdTrip { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("id_accommodation_offer")]
    public int? IdAccommodationOffer { get; set; }

    [Column("id_transportation_offer")]
    public int? IdTransportationOffer { get; set; }

    [Column("id_accommodation_request")]
    public int? IdAccommodationRequest { get; set; }

    [Column("id_transportation_request")]
    public int? IdTransportationRequest { get; set; }
}