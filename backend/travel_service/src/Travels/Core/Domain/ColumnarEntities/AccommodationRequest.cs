using Cassandra;
using Cassandra.Mapping.Attributes;

namespace Travel_Service.src.Travels.Core.Domain.ColumnarEntities;

[Table("accommodation_request", CaseSensitive = true)]
public class AccommodationRequest
{
    [PartitionKey]
    [Column("accommodation_type")]
    public string AccommodationType { get; set; } = string.Empty;

    [ClusteringKey(0)]
    [Column("check_in_date")]
    public LocalDate? CheckInDate { get; set; }

    [ClusteringKey(1)]
    [Column("id_request")]
    public int IdRequest { get; set; }

    [Column("number_of_guests")]
    public int? NumberOfGuests { get; set; }

    [Column("number_of_rooms")]
    public int? NumberOfRooms { get; set; }

    [Column("check_out_date")]
    public LocalDate? CheckOutDate { get; set; }
}