using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.CreditCards.GetAllCreditCards;

public class GetAllCreditCardsHandler : IRequestHandler<GetAllCreditCardsQuery, Result<List<GetAllCreditCardsResponse>>>
{
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly ICreditCardEncryptionService _encryptionService;

    public GetAllCreditCardsHandler(ICreditCardRepository creditCardRepository, ICreditCardEncryptionService encryptionService)
    {
        _creditCardRepository = creditCardRepository;
        _encryptionService = encryptionService;
    }

    public Task<Result<List<GetAllCreditCardsResponse>>> Handle(GetAllCreditCardsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var creditCards = _creditCardRepository.GetAll();

            var response = creditCards.Select(cc => new GetAllCreditCardsResponse
            {
                IdCreditCard = cc.IdCreditCard,
                CreatedAt = cc.CreatedAt,
                Number = _encryptionService.MaskCardNumber(_encryptionService.DecryptCardNumber(cc.Number ?? "")),
                Cvv = null, // Never return CVV for security
                Name = cc.Name,
                ExpirationDate = cc.ExpirationDate,
                IdUser = cc.IdUser,
                UserName = cc.IdUserNavigation?.Name,
                UserEmail = cc.IdUserNavigation?.Email
            }).ToList();

            return Task.FromResult(Result<List<GetAllCreditCardsResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllCreditCardsResponse>>.Failure($"An error occurred while retrieving credit cards: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
