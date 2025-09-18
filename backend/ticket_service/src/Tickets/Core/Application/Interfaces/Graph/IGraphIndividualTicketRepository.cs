using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

public interface IGraphIndividualTicketRepository
{
    public Task CreateIndividualTicket(IndividualTicket ticket);
    public Task<IndividualTicket?> GetIndividualTicketByName(string name);
    public Task<List<IndividualTicket>> GetAllIndividualTickets();
    public Task UpdateIndividualTicket(IndividualTicket ticket);
    public Task DeleteIndividualTicket(string name);
    public Task<List<IndividualTicket>> GetTicketsByType(string type);
    public Task<List<IndividualTicket>> GetTicketsByDateRange(DateTime startDate, DateTime endDate);
}