using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.CreditCards.GetCreditCardsByUser;

public class GetCreditCardsByUserHandler : IRequestHandler<GetCreditCardsByUserQuery, Result<List<GetCreditCardsByUserResponse>>>
{
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICreditCardEncryptionService _encryptionService;

    public GetCreditCardsByUserHandler(
        ICreditCardRepository creditCardRepository, 
        IUserRepository userRepository,
        ICreditCardEncryptionService encryptionService)
    {
        _creditCardRepository = creditCardRepository;
        _userRepository = userRepository;
        _encryptionService = encryptionService;
    }

    public Task<Result<List<GetCreditCardsByUserResponse>>> Handle(GetCreditCardsByUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate that user exists
            var user = _userRepository.GetById(request.IdUser);
            if (user == null)
            {
                return Task.FromResult(Result<List<GetCreditCardsByUserResponse>>.Failure($"User with ID {request.IdUser} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var creditCards = _creditCardRepository.GetByUserId(request.IdUser);

            var response = creditCards.Select(cc => new GetCreditCardsByUserResponse
            {
                IdCreditCard = cc.IdCreditCard,
                CreatedAt = cc.CreatedAt,
                Number = _encryptionService.MaskCardNumber(_encryptionService.DecryptCardNumber(cc.Number ?? "")),
                Name = cc.Name,
                ExpirationDate = cc.ExpirationDate,
                IdUser = cc.IdUser
            }).ToList();

            return Task.FromResult(Result<List<GetCreditCardsByUserResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetCreditCardsByUserResponse>>.Failure($"An error occurred while retrieving credit cards for user: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
