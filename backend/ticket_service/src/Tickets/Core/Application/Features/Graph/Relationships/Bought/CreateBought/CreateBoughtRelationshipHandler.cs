using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.Bought.CreateBought;

public class CreateBoughtRelationshipHandler : IRequestHandler<CreateBoughtRelationshipCommand, Result<CreateBoughtRelationshipResponse>>
{
    private readonly IGraphBoughtRelationshipRepository _boughtRelationshipRepository;

    public CreateBoughtRelationshipHandler(IGraphBoughtRelationshipRepository boughtRelationshipRepository)
    {
        _boughtRelationshipRepository = boughtRelationshipRepository;
    }

    public async Task<Result<CreateBoughtRelationshipResponse>> Handle(CreateBoughtRelationshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.CustomerId <= 0)
            {
                return Result<CreateBoughtRelationshipResponse>.Failure("Customer ID is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            if (request.IndividualTicketId <= 0)
            {
                return Result<CreateBoughtRelationshipResponse>.Failure("Individual Ticket ID is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            if (request.Price < 0)
            {
                return Result<CreateBoughtRelationshipResponse>.Failure("Price cannot be negative")
                    .WithCode((int)ResultCode.BadRequest);
            }

            // Check if relationship already exists
            var existingRelationship = await _boughtRelationshipRepository.GetBoughtRelationship(request.CustomerId, request.IndividualTicketId);
            if (existingRelationship != null)
            {
                return Result<CreateBoughtRelationshipResponse>.Failure("Customer has already bought this ticket")
                    .WithCode((int)ResultCode.Conflict);
            }

            var relationship = await _boughtRelationshipRepository.CreateBoughtRelationship(
                request.CustomerId, 
                request.IndividualTicketId, 
                request.PurchasedAt, 
                request.Price);

            if (relationship == null)
            {
                return Result<CreateBoughtRelationshipResponse>.Failure("Failed to create bought relationship. This ticket may already be bought by another customer.")
                    .WithCode((int)ResultCode.Conflict);
            }

            var response = new CreateBoughtRelationshipResponse
            {
                CustomerId = relationship.CustomerId,
                IndividualTicketId = relationship.IndividualTicketId,
                RelationshipElementId = relationship.RelationshipElementId,
                PurchasedAt = relationship.PurchasedAt,
                Price = relationship.Price
            };

            return Result<CreateBoughtRelationshipResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CreateBoughtRelationshipResponse>.Failure($"An error occurred while creating the bought relationship: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}