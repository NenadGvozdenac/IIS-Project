using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

public interface IGraphCustomerRepository
{
    public Task CreateCustomer(Customer customer);
    public Task<Customer?> GetCustomerByEmail(string email);
    public Task<List<Customer>> GetAllCustomers();
    public Task UpdateCustomer(Customer customer);
    public Task DeleteCustomer(string email);
}