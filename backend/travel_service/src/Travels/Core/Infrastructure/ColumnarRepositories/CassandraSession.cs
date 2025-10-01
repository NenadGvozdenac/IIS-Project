using Cassandra;
using Cassandra.Mapping;

namespace Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories;

public interface ICassandraSession
{
    Cassandra.ISession Session { get; }
    IMapper Mapper { get; }
}

public class CassandraSession : ICassandraSession, IDisposable
{
    private readonly Cluster _cluster;
    private readonly Cassandra.ISession _session;
    private readonly IMapper _mapper;

    public Cassandra.ISession Session => _session;
    public IMapper Mapper => _mapper;

    public CassandraSession(string contactPoint, int port = 9042, string keyspace = "travel_analytics")
    {
        try
        {
            _cluster = Cluster.Builder()
                .AddContactPoint(contactPoint)
                .WithPort(port)
                .WithCredentials("cassandra", "cassandra")
                .Build();

            
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    _session = _cluster.Connect();
                    Console.WriteLine($"[DEBUG] Connected to Cassandra successfully on attempt {i+1}");
                    break;
                }
                catch (Exception connectEx)
                {
                    Console.WriteLine($"[DEBUG] Connection attempt {i+1} failed: {connectEx.Message}");
                    if (i == 4) throw; 
                    System.Threading.Thread.Sleep(2000); 
                }
            }
            
            CreateKeyspaceIfNotExists(keyspace);
            Console.WriteLine($"[DEBUG] Keyspace {keyspace} created/verified");
            
            _session.Execute($"USE {keyspace}");
            Console.WriteLine($"[DEBUG] Using keyspace {keyspace}");
            
            CreateTablesIfNotExist();
            Console.WriteLine($"[DEBUG] Tables created/verified");
            
            _mapper = new Mapper(_session);
            Console.WriteLine($"[DEBUG] Cassandra initialization complete");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to initialize Cassandra: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private void CreateKeyspaceIfNotExists(string keyspace)
    {
        var createKeyspaceQuery = $@"
            CREATE KEYSPACE IF NOT EXISTS {keyspace}
            WITH replication = {{
                'class': 'SimpleStrategy',
                'replication_factor': 1
            }}";
        
        _session.Execute(createKeyspaceQuery);
    }

    private void CreateTablesIfNotExist()
    {
        var createMatchTable = @"
            CREATE TABLE IF NOT EXISTS match (
                city TEXT,
                type TEXT,
                scheduled_at TIMESTAMP,
                id_match INT,
                name TEXT,
                state TEXT,
                hall TEXT,
                is_in_our_hall BOOLEAN,
                transportation_required BOOLEAN,
                accommodation_required BOOLEAN,
                PRIMARY KEY ((city, type), scheduled_at, id_match)
            ) WITH CLUSTERING ORDER BY (scheduled_at DESC, id_match ASC)";

        var createOfferTable = @"
            CREATE TABLE IF NOT EXISTS offer (
                id_match INT,
                type TEXT,
                price INT,
                id_offer INT,
                user_id_user INT,
                id_request INT,
                chosen BOOLEAN,
                score DECIMAL,
                PRIMARY KEY (id_match, type, chosen, price, id_offer)
            ) WITH CLUSTERING ORDER BY (type ASC, chosen ASC, price ASC, id_offer ASC)";

        var createAccommodationRequestTable = @"
            CREATE TABLE IF NOT EXISTS accommodation_request (
                accommodation_type TEXT,
                check_in_date DATE,
                id_request INT,
                number_of_guests INT,
                number_of_rooms INT,
                check_out_date DATE,
                PRIMARY KEY (accommodation_type, check_in_date, id_request)
            ) WITH CLUSTERING ORDER BY (check_in_date DESC, id_request ASC)";

        var createTransportationRequestTable = @"
            CREATE TABLE IF NOT EXISTS transportation_request (
                vehicle_type TEXT,
                start_date DATE,
                id_request INT,
                number_of_passengers INT,
                end_date DATE,
                PRIMARY KEY (vehicle_type, start_date, id_request)
            ) WITH CLUSTERING ORDER BY (start_date DESC, id_request ASC)";

        var createAccommodationOfferTable = @"
            CREATE TABLE IF NOT EXISTS accommodation_offer (
                accommodation_type TEXT,
                capacity INT,
                id_offer INT,
                name TEXT,
                id_request INT,
                double_room BOOLEAN,
                triple_room BOOLEAN,
                quadruple_room BOOLEAN,
                breakfast BOOLEAN,
                fitness_center BOOLEAN,
                pool BOOLEAN,
                wifi BOOLEAN,
                spa BOOLEAN,
                PRIMARY KEY (accommodation_type, capacity, id_offer)
            ) WITH CLUSTERING ORDER BY (capacity DESC, id_offer ASC)";

        var createTransportationOfferTable = @"
            CREATE TABLE IF NOT EXISTS transportation_offer (
                type TEXT,
                capacity INT,
                id_offer INT,
                company_name TEXT,
                id_request INT,
                equipment_space BOOLEAN,
                air_conditioning BOOLEAN,
                tv BOOLEAN,
                wifi BOOLEAN,
                restroom BOOLEAN,
                PRIMARY KEY (type, capacity, id_offer)
            ) WITH CLUSTERING ORDER BY (capacity DESC, id_offer ASC)";

        var createTripTable = @"
            CREATE TABLE IF NOT EXISTS trip (
                match_id_match INT,
                id_trip INT,
                notes TEXT,
                id_accommodation_offer INT,
                id_transportation_offer INT,
                id_accommodation_request INT,
                id_transportation_request INT,
                PRIMARY KEY (match_id_match, id_trip)
            ) WITH CLUSTERING ORDER BY (id_trip ASC)";

        var createRequestTable = @"
            CREATE TABLE IF NOT EXISTS request (
                id_match INT,
                type TEXT,
                budget INT,
                id_request INT,
                state TEXT,
                city TEXT,
                hall TEXT,
                PRIMARY KEY (id_match, type, budget, id_request)
            ) WITH CLUSTERING ORDER BY (type ASC, budget ASC, id_request ASC)";

        _session.Execute(createMatchTable);
        _session.Execute(createOfferTable);
        _session.Execute(createAccommodationRequestTable);
        _session.Execute(createTransportationRequestTable);
        _session.Execute(createAccommodationOfferTable);
        _session.Execute(createTransportationOfferTable);
        _session.Execute(createTripTable);
        _session.Execute(createRequestTable);
    }

    public void Dispose()
    {
        _session?.Dispose();
        _cluster?.Dispose();
    }
}