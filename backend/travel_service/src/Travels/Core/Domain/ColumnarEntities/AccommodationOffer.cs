using Cassandra.Mapping.Attributes;

namespace Travel_Service.src.Travels.Core.Domain.ColumnarEntities;

[Table("accommodation_offer", CaseSensitive = true)]
public class AccommodationOffer
{
    [PartitionKey]
    [Column("accommodation_type")]
    public string AccommodationType { get; set; } = string.Empty;

    [ClusteringKey(0)]
    [Column("capacity")]
    public int Capacity { get; set; }

    [ClusteringKey(1)]
    [Column("id_offer")]
    public int IdOffer { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("id_request")]
    public int IdRequest { get; set; }

    [Column("double_room")]
    public bool DoubleRoom { get; set; }

    [Column("triple_room")]
    public bool TripleRoom { get; set; }

    [Column("quadruple_room")]
    public bool QuadrupleRoom { get; set; }

    [Column("breakfast")]
    public bool Breakfast { get; set; }

    [Column("fitness_center")]
    public bool FitnessCenter { get; set; }

    [Column("pool")]
    public bool Pool { get; set; }

    [Column("wifi")]
    public bool Wifi { get; set; }

    [Column("spa")]
    public bool Spa { get; set; }
}