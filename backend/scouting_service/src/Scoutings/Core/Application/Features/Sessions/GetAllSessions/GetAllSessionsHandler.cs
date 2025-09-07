using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Sessions.GetAllSessions;

public class GetAllSessionsHandler : IRequestHandler<GetAllSessionsQuery, Result<List<GetAllSessionsResponse>>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetAllSessionsHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public Task<Result<List<GetAllSessionsResponse>>> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var sessions = _sessionRepository.GetAll();

            var response = sessions.Select(s => new GetAllSessionsResponse
            {
                IdSession = s.IdSession,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                IdSessionStatus = s.IdSessionStatus,
                SessionStatusName = s.IdSessionStatusNavigation?.Status,
                IdSessionType = s.IdSessionType,
                SessionTypeName = s.IdSessionTypeNavigation?.Type,
                IdUser = s.IdUser,
                UserName = s.IdUserNavigation?.Name,
                IdPlayer = s.IdPlayer,
                PlayerName = s.IdPlayerNavigation != null ? $"{s.IdPlayerNavigation.Name} {s.IdPlayerNavigation.Surname}" : null,
                Note = s.Note
            }).ToList();

            return Task.FromResult(Result<List<GetAllSessionsResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllSessionsResponse>>.Failure($"An error occurred while retrieving sessions: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
