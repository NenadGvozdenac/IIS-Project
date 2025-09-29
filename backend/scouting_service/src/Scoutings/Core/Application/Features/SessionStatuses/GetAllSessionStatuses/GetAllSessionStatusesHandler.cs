using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionStatuses.GetAllSessionStatuses;

public class GetAllSessionStatusesHandler : IRequestHandler<GetAllSessionStatusesQuery, Result<GetAllSessionStatusesResponse>>
{
    private readonly ISessionStatusRepository _sessionStatusRepository;

    public GetAllSessionStatusesHandler(ISessionStatusRepository sessionStatusRepository)
    {
        _sessionStatusRepository = sessionStatusRepository;
    }

    public Task<Result<GetAllSessionStatusesResponse>> Handle(GetAllSessionStatusesQuery request, CancellationToken cancellationToken)
    {
        var sessionStatuses = _sessionStatusRepository.GetAll();

        var sessionStatusDtos = sessionStatuses.Select(ss => new SessionStatusDto
        {
            Id = ss.IdStatus,
            Name = ss.Status ?? string.Empty
        }).ToList();

        var response = new GetAllSessionStatusesResponse
        {
            SessionStatuses = sessionStatusDtos
        };

        return Task.FromResult(Result<GetAllSessionStatusesResponse>.Success(response));
    }
}
