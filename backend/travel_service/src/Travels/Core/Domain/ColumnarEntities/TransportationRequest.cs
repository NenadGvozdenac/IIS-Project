using Cassandra;
using Cassandra.Mapping.Attributes;

namespace Travel_Service.src.Travels.Core.Domain.ColumnarEntities;

[Table("transportation_request", CaseSensitive = true)]
public class TransportationRequest
{
    [PartitionKey]
    [Column("vehicle_type")]
    public string VehicleType { get; set; } = string.Empty;

    [ClusteringKey(0)]
    [Column("start_date")]
    public LocalDate? StartDate { get; set; }

    [ClusteringKey(1)]
    [Column("id_request")]
    public int IdRequest { get; set; }

    [Column("number_of_passengers")]
    public int? NumberOfPassengers { get; set; }

    [Column("end_date")]
    public LocalDate? EndDate { get; set; }
}