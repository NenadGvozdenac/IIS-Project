using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.CreditCards.GetCreditCardById;

public class GetCreditCardByIdHandler : IRequestHandler<GetCreditCardByIdQuery, Result<GetCreditCardByIdResponse>>
{
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly ICreditCardEncryptionService _encryptionService;

    public GetCreditCardByIdHandler(ICreditCardRepository creditCardRepository, ICreditCardEncryptionService encryptionService)
    {
        _creditCardRepository = creditCardRepository;
        _encryptionService = encryptionService;
    }

    public Task<Result<GetCreditCardByIdResponse>> Handle(GetCreditCardByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var creditCard = _creditCardRepository.GetById(request.IdCreditCard);
            if (creditCard == null)
            {
                return Task.FromResult(Result<GetCreditCardByIdResponse>.Failure($"Credit card with ID {request.IdCreditCard} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetCreditCardByIdResponse
            {
                IdCreditCard = creditCard.IdCreditCard,
                CreatedAt = creditCard.CreatedAt,
                Number = _encryptionService.MaskCardNumber(_encryptionService.DecryptCardNumber(creditCard.Number ?? "")),
                Name = creditCard.Name,
                ExpirationDate = creditCard.ExpirationDate,
                IdUser = creditCard.IdUser,
                UserName = creditCard.IdUserNavigation?.Name,
                UserEmail = creditCard.IdUserNavigation?.Email
            };

            return Task.FromResult(Result<GetCreditCardByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetCreditCardByIdResponse>.Failure($"An error occurred while retrieving the credit card: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
