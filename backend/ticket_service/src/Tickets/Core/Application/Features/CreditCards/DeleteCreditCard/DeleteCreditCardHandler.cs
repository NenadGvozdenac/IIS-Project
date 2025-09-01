using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.CreditCards.DeleteCreditCard;

public class DeleteCreditCardHandler : IRequestHandler<DeleteCreditCardCommand, Result<DeleteCreditCardResponse>>
{
    private readonly ICreditCardRepository _creditCardRepository;

    public DeleteCreditCardHandler(ICreditCardRepository creditCardRepository)
    {
        _creditCardRepository = creditCardRepository;
    }

    public Task<Result<DeleteCreditCardResponse>> Handle(DeleteCreditCardCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var success = _creditCardRepository.Delete(request.IdCreditCard);

            if (!success)
            {
                return Task.FromResult(Result<DeleteCreditCardResponse>.Failure($"Credit card with ID {request.IdCreditCard} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new DeleteCreditCardResponse
            {
                Success = true,
                Message = "Credit card deleted successfully"
            };

            return Task.FromResult(Result<DeleteCreditCardResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<DeleteCreditCardResponse>.Failure($"An error occurred while deleting the credit card: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
