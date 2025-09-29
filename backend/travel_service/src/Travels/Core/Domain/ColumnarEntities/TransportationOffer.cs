using Cassandra.Mapping.Attributes;

namespace Travel_Service.src.Travels.Core.Domain.ColumnarEntities;

[Table("transportation_offer", CaseSensitive = true)]
public class TransportationOffer
{
    [PartitionKey]
    [Column("type")]
    public string Type { get; set; } = string.Empty;

    [ClusteringKey(0)]
    [Column("capacity")]
    public int Capacity { get; set; }

    [ClusteringKey(1)]
    [Column("id_offer")]
    public int IdOffer { get; set; }

    [Column("company_name")]
    public string? CompanyName { get; set; }

    [Column("id_request")]
    public int IdRequest { get; set; }

    [Column("equipment_space")]
    public bool EquipmentSpace { get; set; }

    [Column("air_conditioning")]
    public bool AirConditioning { get; set; }

    [Column("tv")]
    public bool Tv { get; set; }

    [Column("wifi")]
    public bool Wifi { get; set; }

    [Column("restroom")]
    public bool Restroom { get; set; }
}