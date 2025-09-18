using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface INeo4jSeedingService
{
    Task<Result> SeedDatabaseAndRemoveInitContainer();
}