namespace ticket_service.src.Tickets.Core.Application.DTOs.Graph;

// IndividualTicket DTOs
public class CreateIndividualTicketDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime ReleasedAt { get; set; }
    public decimal Price { get; set; }
}

public class UpdateIndividualTicketDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime ReleasedAt { get; set; }
    public decimal Price { get; set; }
}

public class IndividualTicketResponseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime ReleasedAt { get; set; }
    public decimal Price { get; set; }
}