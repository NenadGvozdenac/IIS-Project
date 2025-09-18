using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

public interface IGraphIndividualTicketRepository
{
    public Task<IndividualTicket?> CreateIndividualTicket(IndividualTicket ticket);
    public Task<IndividualTicket?> GetIndividualTicketByName(string name);
    public Task<IndividualTicket?> GetIndividualTicketById(int id);
    public Task<List<IndividualTicket>> GetAllIndividualTickets();
    public Task<IndividualTicket?> UpdateIndividualTicket(int id, IndividualTicket ticket);
    public Task DeleteIndividualTicket(int id);
}