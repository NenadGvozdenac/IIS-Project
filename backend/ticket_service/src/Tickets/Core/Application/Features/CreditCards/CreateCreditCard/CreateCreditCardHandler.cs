using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Features.CreditCards.CreateCreditCard;

public class CreateCreditCardHandler : IRequestHandler<CreateCreditCardCommand, Result<CreateCreditCardResponse>>
{
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICreditCardEncryptionService _encryptionService;

    public CreateCreditCardHandler(
        ICreditCardRepository creditCardRepository, 
        IUserRepository userRepository,
        ICreditCardEncryptionService encryptionService)
    {
        _creditCardRepository = creditCardRepository;
        _userRepository = userRepository;
        _encryptionService = encryptionService;
    }

    public Task<Result<CreateCreditCardResponse>> Handle(CreateCreditCardCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.Number))
            {
                return Task.FromResult(Result<CreateCreditCardResponse>.Failure("Credit card number is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Task.FromResult(Result<CreateCreditCardResponse>.Failure("Cardholder name is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (!request.Cvv.HasValue || request.Cvv.Value < 100 || request.Cvv.Value > 9999)
            {
                return Task.FromResult(Result<CreateCreditCardResponse>.Failure("CVV must be a 3 or 4 digit number")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Validate user exists
            var user = _userRepository.GetById(request.IdUser);
            if (user == null)
            {
                return Task.FromResult(Result<CreateCreditCardResponse>.Failure($"User with ID {request.IdUser} not found")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Validate credit card number format (basic validation)
            var cleanNumber = request.Number.Replace(" ", "").Replace("-", "");
            if (cleanNumber.Length < 13 || cleanNumber.Length > 19 || !cleanNumber.All(char.IsDigit))
            {
                return Task.FromResult(Result<CreateCreditCardResponse>.Failure("Invalid credit card number format")
                    .WithCode((int)ResultCode.BadRequest));
            }

            var creditCard = new CreditCard
            {
                Number = _encryptionService.EncryptCardNumber(cleanNumber),
                Cvv = _encryptionService.EncryptCvv(request.Cvv.Value.ToString()),
                Name = request.Name,
                ExpirationDate = request.ExpirationDate,
                IdUser = request.IdUser,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            var createdCreditCard = _creditCardRepository.Create(creditCard);

            var response = new CreateCreditCardResponse
            {
                IdCreditCard = createdCreditCard.IdCreditCard,
                CreatedAt = createdCreditCard.CreatedAt,
                Number = _encryptionService.MaskCardNumber(_encryptionService.DecryptCardNumber(createdCreditCard.Number ?? "")),
                Name = createdCreditCard.Name,
                ExpirationDate = createdCreditCard.ExpirationDate,
                IdUser = createdCreditCard.IdUser
            };

            return Task.FromResult(Result<CreateCreditCardResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateCreditCardResponse>.Failure($"An error occurred while creating the credit card: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
