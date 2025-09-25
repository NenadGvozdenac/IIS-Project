using Cassandra;
using Cassandra.Mapping;
using Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories;
using Travel_Service.src.Travels.Core.Domain.ColumnarEntities;

namespace Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories;

public class MatchRepository : BaseColumnarRepository<Match>, IMatchRepository
{
    public MatchRepository(Cassandra.ISession session, IMapper mapper) 
        : base(session, mapper, "match") { }

    protected override string GetPrimaryKeyColumn() => "id_match";

    protected override string GetWhereClause(int keyCount)
    {
        return keyCount switch
        {
            1 => "id_match = ?",
            4 => "city = ? and type = ? and scheduled_at = ? and id_match = ?",
            _ => throw new ArgumentException("Match requires exactly 4 keys: city, type, scheduled_at, id_match")
        };
    }

    protected override object[] GetEntityKeyValues(Match entity)
    {
        return new object[] { entity.IdMatch };
    }
}

public class RequestRepository : BaseColumnarRepository<Request>, IRequestRepository
{
    public RequestRepository(Cassandra.ISession session, IMapper mapper) 
        : base(session, mapper, "request") { }

    protected override string GetPrimaryKeyColumn() => "id_match";

    protected override string GetWhereClause(int keyCount)
    {
        return keyCount switch
        {
            1 => "id_request = ?",
            2 => "id_match = ? AND id_request = ? ",
            4 => "id_match = ? AND type = ? AND budget = ? AND id_request = ? ",
            _ => throw new ArgumentException("Request requires exactly 4 keys: id_match, id_request, type, budget")
        };
    }

    protected override object[] GetEntityKeyValues(Request entity)
    {
        return new object[] { entity.IdMatch, entity.IdRequest };
    }
}

public class AccommodationRequestRepository : BaseColumnarRepository<AccommodationRequest>, IAccommodationRequestRepository
{
    public AccommodationRequestRepository(Cassandra.ISession session, IMapper mapper) 
        : base(session, mapper, "accommodation_request") { }

    protected override string GetPrimaryKeyColumn() => "id_request";

    protected override string GetWhereClause(int keyCount)
    {
        return keyCount switch
        {
            1 => "id_request = ?",
            3 => "accommodation_type = ? AND check_in_date = ? AND id_request = ?",
            _ => throw new ArgumentException("AccommodationRequest requires exactly 1 key: id_request")
        };
    }

    protected override object[] GetEntityKeyValues(AccommodationRequest entity)
    {
        return new object[] { entity.IdRequest };
    }
}