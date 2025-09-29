using Cassandra;
using Cassandra.Mapping;
using Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories;
using Travel_Service.src.Travels.Core.Domain.ColumnarEntities;

namespace Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories;

public class TransportationRequestRepository : BaseColumnarRepository<TransportationRequest>, ITransportationRequestRepository
{
    public TransportationRequestRepository(Cassandra.ISession session, IMapper mapper) 
        : base(session, mapper, "transportation_request") { }

    protected override string GetPrimaryKeyColumn() => "id_request";

    protected override string GetWhereClause(int keyCount)
    {
        return keyCount switch
        {
            1 => "id_request = ?",
            3 => "vehicle_type = ? AND start_date = ? AND id_request = ?",
            _ => throw new ArgumentException("TransportationRequest requires exactly 3 keys: vehicle_type, start_date, id_request")
        };
    }

    protected override object[] GetEntityKeyValues(TransportationRequest entity)
    {
        return new object[] { entity.IdRequest };
    }
}

public class OfferRepository : BaseColumnarRepository<Offer>, IOfferRepository
{
    public OfferRepository(Cassandra.ISession session, IMapper mapper) 
        : base(session, mapper, "offer") { }

    protected override string GetPrimaryKeyColumn() => "id_match";

    protected override string GetWhereClause(int keyCount)
    {
        return keyCount switch
        {
            2 => "id_request = ? AND id_offer = ?",
            5 => "id_match = ? and type = ? and chosen = ? and price = ? and id_offer = ?",
            _ => throw new ArgumentException("Offer requires exactly 5 keys: id_match, type, chosen, price, id_offer")
        };
    }

    protected override object[] GetEntityKeyValues(Offer entity)
    {
        return new object[] { entity.IdMatch, entity.IdOffer, entity.IdRequest };
    }
}

public class AccommodationOfferRepository : BaseColumnarRepository<AccommodationOffer>, IAccommodationOfferRepository
{
    public AccommodationOfferRepository(Cassandra.ISession session, IMapper mapper) 
        : base(session, mapper, "accommodation_offer") { }

    protected override string GetPrimaryKeyColumn() => "id_offer";

    protected override string GetWhereClause(int keyCount)
    {
        return keyCount switch
        {
            2 => "id_request = ? AND id_offer = ?",
            3 => "accommodation_type = ? AND capacity = ? AND id_offer = ?",
            _ => throw new ArgumentException("AccommodationOffer requires exactly 3 keys: accommodation_type, capacity, id_offer")
        };
    }

    protected override object[] GetEntityKeyValues(AccommodationOffer entity)
    {
        return new object[] { entity.IdOffer, entity.IdRequest };
    }
}

public class TransportationOfferRepository : BaseColumnarRepository<TransportationOffer>, ITransportationOfferRepository
{
    public TransportationOfferRepository(Cassandra.ISession session, IMapper mapper) 
        : base(session, mapper, "transportation_offer") { }

    protected override string GetPrimaryKeyColumn() => "id_offer";

    protected override string GetWhereClause(int keyCount)
    {
        return keyCount switch
        {
            2 => "id_request = ? AND id_offer = ?",
            3 => "type = ? AND capacity = ? AND id_offer = ?",
            _ => throw new ArgumentException("TransportationOffer requires exactly 3 keys: type, capacity, id_offer")
        };
    }

    protected override object[] GetEntityKeyValues(TransportationOffer entity)
    {
        return new object[] { entity.IdOffer, entity.IdRequest };
    }
}

public class TripRepository : BaseColumnarRepository<Trip>, ITripRepository
{
    public TripRepository(Cassandra.ISession session, IMapper mapper) 
        : base(session, mapper, "trip") { }

    protected override string GetPrimaryKeyColumn() => "match_id_match";

    protected override string GetWhereClause(int keyCount)
    {
        return keyCount switch
        {
            2 => "match_id_match = ? AND id_trip = ? ",
            _ => throw new ArgumentException("Trip requires exactly 2 keys: match_id_match, id_trip")
        };
    }

    protected override object[] GetEntityKeyValues(Trip entity)
    {
        return new object[] { entity.MatchIdMatch, entity.IdTrip, entity.IdAccommodationOffer, entity.IdTransportationOffer, entity.IdAccommodationRequest, entity.IdTransportationRequest };
    }
}