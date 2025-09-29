using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Features.Visas.CreateVisa;

public class CreateVisaHandler : IRequestHandler<CreateVisaCommand, Result<CreateVisaResponse>>
{
    private readonly IVisaRepository _visaRepository;
    private readonly IUserRepository _userRepository;

    public CreateVisaHandler(IVisaRepository visaRepository, IUserRepository userRepository)
    {
        _visaRepository = visaRepository;
        _userRepository = userRepository;
    }

    public Task<Result<CreateVisaResponse>> Handle(CreateVisaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = _userRepository.GetById(request.UserId);
            if (user == null)
            {
                return Task.FromResult(Result<CreateVisaResponse>.Failure("User not found")
                    .WithCode((int)ResultCode.NotFound));
            }
            if (user.Type != "team manager")
            {
                return Task.FromResult(Result<CreateVisaResponse>.Failure("Only team manager can create visas")
                    .WithCode((int)ResultCode.Forbidden));
            }
            if (string.IsNullOrWhiteSpace(request.VisaNumber))
            {
                return Task.FromResult(Result<CreateVisaResponse>.Failure("Visa number is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (string.IsNullOrWhiteSpace(request.State))
            {
                return Task.FromResult(Result<CreateVisaResponse>.Failure("State is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (request.CreationDate is null)
            {
                return Task.FromResult(Result<CreateVisaResponse>.Failure("Creation date is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (request.ExpirationDate is null)
            {
                return Task.FromResult(Result<CreateVisaResponse>.Failure("Expiration date is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (request.IdTravelInformation == 0)
            {
                return Task.FromResult(Result<CreateVisaResponse>.Failure("Travel information ID is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            var visa = new Visa
            {
                VisaNumber = request.VisaNumber,
                State = request.State,
                CreationDate = request.CreationDate,
                ExpirationDate = request.ExpirationDate,
                IdTravelInformation = request.IdTravelInformation
            };

            var createdVisa = _visaRepository.Create(visa);

            var response = new CreateVisaResponse
            {
                VisaNumber = createdVisa.VisaNumber,
                State = createdVisa.State,
                CreationDate = createdVisa.CreationDate,
                ExpirationDate = createdVisa.ExpirationDate,
                IdTravelInformation = createdVisa.IdTravelInformation
            };

            return Task.FromResult(Result<CreateVisaResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateVisaResponse>.Failure($"An error occurred while creating the visa: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}