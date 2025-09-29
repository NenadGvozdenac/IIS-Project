using Microsoft.EntityFrameworkCore;
using MediatR;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Infrastructure.Repositories;
using System.Reflection;
using travel_service.src.Travels.Core.Infrastructure;
using Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories;
using Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories;
using Cassandra.Mapping;

namespace travel_service.src.Travels.API.Startup;

public static class ApplicationStartup
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        SetupDatabases(services, configuration);
        SetupRepositories(services);
        SetupColumnarRepositories(services, configuration);
        SetupMediatR(services);

        return services;
    }

    private static void SetupRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<travel_service.src.Travels.Core.Application.Interfaces.IMatchRepository, travel_service.src.Travels.Core.Infrastructure.Repositories.MatchRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<ITravelInfoRepository, TravelInfoRepository>();
        services.AddScoped<IVisaRepository, VisaRepository>();
        services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
        services.AddScoped<ICompetitionRepository, CompetitionRepository>();
        services.AddScoped<INationalityRepository, NationalityRepository>();
        services.AddScoped<IAgenciesRepository, AgenciesRepository>();
        services.AddScoped<IRequestsRepository, RequestsRepository>();
        services.AddScoped<IOffersRepository, OffersRepository>();
        services.AddScoped<travel_service.src.Travels.Core.Application.Interfaces.ITripRepository, travel_service.src.Travels.Core.Infrastructure.Repositories.TripRepository>();
    }

    private static void SetupColumnarRepositories(IServiceCollection services, IConfiguration configuration)
    {
        Console.WriteLine("[DEBUG] Starting Cassandra setup...");
        
        // Configure Cassandra
        var cassandraHost = configuration.GetValue<string>("Cassandra:Host") ?? "cassandra";
        var cassandraPort = configuration.GetValue<int>("Cassandra:Port", 9042);
        var cassandraKeyspace = configuration.GetValue<string>("Cassandra:Keyspace") ?? "travel_analytics";

        Console.WriteLine($"[DEBUG] Cassandra config: Host={cassandraHost}, Port={cassandraPort}, Keyspace={cassandraKeyspace}");

        services.AddSingleton<Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories.ICassandraSession>(provider => 
        {
            try 
            {
                Console.WriteLine("[DEBUG] Creating CassandraSession instance...");
                var cassandraSession = new Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories.CassandraSession(cassandraHost, cassandraPort, cassandraKeyspace);
                Console.WriteLine("[DEBUG] CassandraSession instance created successfully");
                return cassandraSession;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to create CassandraSession: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                throw;
            }
        });
        
        Console.WriteLine("[DEBUG] Registered ICassandraSession");
        
        // Register Cassandra.ISession from ICassandraSession
        services.AddSingleton<Cassandra.ISession>(provider => 
        {
            try 
            {
                Console.WriteLine("[DEBUG] Getting ICassandraSession to extract Cassandra.ISession...");
                var cassandraSession = provider.GetRequiredService<Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories.ICassandraSession>();
                Console.WriteLine("[DEBUG] Got ICassandraSession, extracting Session property...");
                var session = cassandraSession.Session;
                Console.WriteLine("[DEBUG] Successfully extracted Cassandra.ISession");
                return session;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get Cassandra.ISession: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                throw;
            }
        });
        
        Console.WriteLine("[DEBUG] Registered Cassandra.ISession");
        
        // Register IMapper for Cassandra
        services.AddSingleton<Cassandra.Mapping.IMapper>(provider => 
            new Cassandra.Mapping.Mapper(provider.GetRequiredService<Cassandra.ISession>()));
        
        Console.WriteLine("[DEBUG] Registered Cassandra Mappers");
        
        // Register Cassandra repositories directly as interfaces
        try
        {
            Console.WriteLine("[DEBUG] Registering IMatchRepository...");
            services.AddScoped<Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories.IMatchRepository, Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories.MatchRepository>();
            Console.WriteLine("[DEBUG] IMatchRepository registered successfully");
            
            Console.WriteLine("[DEBUG] Registering IRequestRepository...");
            services.AddScoped<Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories.IRequestRepository, Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories.RequestRepository>();
            Console.WriteLine("[DEBUG] IRequestRepository registered successfully");
            
            Console.WriteLine("[DEBUG] Registering IOfferRepository...");
            services.AddScoped<Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories.IOfferRepository, Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories.OfferRepository>();
            Console.WriteLine("[DEBUG] IOfferRepository registered successfully");
            
            Console.WriteLine("[DEBUG] Registering IAccommodationRequestRepository...");
            services.AddScoped<Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories.IAccommodationRequestRepository, Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories.AccommodationRequestRepository>();
            Console.WriteLine("[DEBUG] IAccommodationRequestRepository registered successfully");
            
            Console.WriteLine("[DEBUG] Registering ITransportationRequestRepository...");
            services.AddScoped<Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories.ITransportationRequestRepository, Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories.TransportationRequestRepository>();
            Console.WriteLine("[DEBUG] ITransportationRequestRepository registered successfully");
            
            Console.WriteLine("[DEBUG] Registering IAccommodationOfferRepository...");
            services.AddScoped<Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories.IAccommodationOfferRepository, Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories.AccommodationOfferRepository>();
            Console.WriteLine("[DEBUG] IAccommodationOfferRepository registered successfully");
            
            Console.WriteLine("[DEBUG] Registering ITransportationOfferRepository...");
            services.AddScoped<Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories.ITransportationOfferRepository, Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories.TransportationOfferRepository>();
            Console.WriteLine("[DEBUG] ITransportationOfferRepository registered successfully");
            
            Console.WriteLine("[DEBUG] Registering ITripRepository...");
            services.AddScoped<Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories.ITripRepository, Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories.TripRepository>();
            Console.WriteLine("[DEBUG] ITripRepository registered successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to register repository: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            throw;
        }
        
        Console.WriteLine("[DEBUG] Registered all Cassandra repositories");
    }

    private static void SetupMediatR(IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        
        // Registruj current assembly
        var currentAssembly = typeof(ApplicationStartup).Assembly;
        services.AddMediatR(currentAssembly);
    }
    
    private static void SetupDatabases(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            "Host=postgres_db;Database=sportsdb;Username=postgres;Password=postgres;Port=5432";
        
        services.AddDbContext<TravelDbContext>(options =>
          options.UseNpgsql(connectionString));
    }
}