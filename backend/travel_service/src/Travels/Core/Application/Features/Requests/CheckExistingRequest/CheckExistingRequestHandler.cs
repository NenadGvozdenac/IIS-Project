using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Requests.CheckExistingRequest;

public class CheckExistingRequestHandler : IRequestHandler<CheckExistingRequestQuery, Result<CheckExistingRequestResponse>>
{
    private readonly IRequestsRepository _requestsRepository;

    public CheckExistingRequestHandler(IRequestsRepository requestsRepository)
    {
        _requestsRepository = requestsRepository;
    }

    public Task<Result<CheckExistingRequestResponse>> Handle(CheckExistingRequestQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var existingRequest = _requestsRepository.GetRequestByMatchIdAndType(request.MatchId, request.Type);
            
            var response = new CheckExistingRequestResponse
            {
                RequestExists = existingRequest != null,
                ExistingRequestId = existingRequest?.IdRequest
            };

            return Task.FromResult(Result<CheckExistingRequestResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CheckExistingRequestResponse>.Failure($"An error occurred while checking for existing request: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}