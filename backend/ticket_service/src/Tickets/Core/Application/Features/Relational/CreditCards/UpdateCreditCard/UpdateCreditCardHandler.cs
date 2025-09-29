using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.CreditCards.UpdateCreditCard;

public class UpdateCreditCardHandler : IRequestHandler<UpdateCreditCardCommand, Result<UpdateCreditCardResponse>>
{
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly ICreditCardEncryptionService _encryptionService;

    public UpdateCreditCardHandler(ICreditCardRepository creditCardRepository, ICreditCardEncryptionService encryptionService)
    {
        _creditCardRepository = creditCardRepository;
        _encryptionService = encryptionService;
    }

    public Task<Result<UpdateCreditCardResponse>> Handle(UpdateCreditCardCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingCreditCard = _creditCardRepository.GetById(request.IdCreditCard);
            if (existingCreditCard == null)
            {
                return Task.FromResult(Result<UpdateCreditCardResponse>.Failure($"Credit card with ID {request.IdCreditCard} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Task.FromResult(Result<UpdateCreditCardResponse>.Failure("Cardholder name is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (!request.Cvv.HasValue || request.Cvv.Value < 100 || request.Cvv.Value > 9999)
            {
                return Task.FromResult(Result<UpdateCreditCardResponse>.Failure("CVV must be a 3 or 4 digit number")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Validate credit card number format if provided
            if (!string.IsNullOrWhiteSpace(request.Number))
            {
                var cleanNumber = request.Number.Replace(" ", "").Replace("-", "");
                if (cleanNumber.Length < 13 || cleanNumber.Length > 19 || !cleanNumber.All(char.IsDigit))
                {
                    return Task.FromResult(Result<UpdateCreditCardResponse>.Failure("Invalid credit card number format")
                        .WithCode((int)ResultCode.BadRequest));
                }
                existingCreditCard.Number = _encryptionService.EncryptCardNumber(cleanNumber);
            }

            if (!request.ExpirationDate.HasValue)
            {
                return Task.FromResult(Result<UpdateCreditCardResponse>.Failure("Expiration date is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            existingCreditCard.Cvv = _encryptionService.EncryptCvv(request.Cvv.Value.ToString());
            existingCreditCard.Name = request.Name ?? existingCreditCard.Name;
            existingCreditCard.ExpirationDate = request.ExpirationDate.Value;

            var updatedCreditCard = _creditCardRepository.Update(existingCreditCard);

            var response = new UpdateCreditCardResponse
            {
                IdCreditCard = updatedCreditCard.IdCreditCard,
                CreatedAt = updatedCreditCard.CreatedAt,
                Number = _encryptionService.MaskCardNumber(_encryptionService.DecryptCardNumber(updatedCreditCard.Number ?? "")),
                Name = updatedCreditCard.Name,
                ExpirationDate = updatedCreditCard.ExpirationDate,
                IdUser = updatedCreditCard.IdUser
            };

            return Task.FromResult(Result<UpdateCreditCardResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateCreditCardResponse>.Failure($"An error occurred while updating the credit card: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
