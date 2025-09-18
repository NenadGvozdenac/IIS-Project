using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Infrastructure.Database;
using Neo4j.Driver;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph;

public class CustomerRepository : IGraphCustomerRepository
{
    private readonly IGraphDatabaseContext _graphDbContext;

    public CustomerRepository(IGraphDatabaseContext graphDbContext)
    {
        _graphDbContext = graphDbContext;
    }

    public async Task CreateCustomer(Customer customer)
    {
        var query = @"
            CREATE (c:Customer {
                email: $email,
                name: $name,
                surname: $surname,
                phone: $phone,
                type: $type
            })";

        var parameters = new
        {
            email = customer.Email,
            name = customer.Name,
            surname = customer.Surname,
            phone = customer.Phone,
            type = customer.Type
        };

        await _graphDbContext.RunAsync(query, parameters);
    }

    public async Task DeleteCustomer(string email)
    {
        var query = @"
            MATCH (c:Customer {email: $email})
            DETACH DELETE c";

        var parameters = new { email };

        await _graphDbContext.RunAsync(query, parameters);
    }

    public async Task<List<Customer>> GetAllCustomers()
    {
        var query = @"
            MATCH (c:Customer)
            RETURN c.email as email, c.name as name, c.surname as surname, 
                   c.phone as phone, c.type as type";

        var result = await _graphDbContext.RunAsync(query);
        var customers = new List<Customer>();

        await foreach (var record in result)
        {
            customers.Add(new Customer
            {
                Email = record["email"].As<string>(),
                Name = record["name"].As<string>(),
                Surname = record["surname"].As<string>(),
                Phone = record["phone"].As<string>(),
                Type = record["type"].As<string>()
            });
        }

        return customers;
    }

    public async Task<Customer?> GetCustomerByEmail(string email)
    {
        var query = @"
            MATCH (c:Customer {email: $email})
            RETURN c.email as email, c.name as name, c.surname as surname, 
                   c.phone as phone, c.type as type";

        var parameters = new { email };
        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new Customer
            {
                Email = record["email"].As<string>(),
                Name = record["name"].As<string>(),
                Surname = record["surname"].As<string>(),
                Phone = record["phone"].As<string>(),
                Type = record["type"].As<string>()
            };
        }

        return null;
    }

    public async Task UpdateCustomer(Customer customer)
    {
        var query = @"
            MATCH (c:Customer {email: $email})
            SET c.name = $name,
                c.surname = $surname,
                c.phone = $phone,
                c.type = $type";

        var parameters = new
        {
            email = customer.Email,
            name = customer.Name,
            surname = customer.Surname,
            phone = customer.Phone,
            type = customer.Type
        };

        await _graphDbContext.RunAsync(query, parameters);
    }
}