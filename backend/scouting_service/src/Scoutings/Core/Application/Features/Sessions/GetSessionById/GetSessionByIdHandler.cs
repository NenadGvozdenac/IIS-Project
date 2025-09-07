using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Sessions.GetSessionById;

public class GetSessionByIdHandler : IRequestHandler<GetSessionByIdQuery, Result<GetSessionByIdResponse>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionByIdHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public Task<Result<GetSessionByIdResponse>> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var session = _sessionRepository.GetById(request.Id);

            if (session == null)
            {
                return Task.FromResult(Result<GetSessionByIdResponse>.Failure($"Session with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetSessionByIdResponse
            {
                IdSession = session.IdSession,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                IdSessionStatus = session.IdSessionStatus,
                SessionStatusName = session.IdSessionStatusNavigation?.Status,
                IdSessionType = session.IdSessionType,
                SessionTypeName = session.IdSessionTypeNavigation?.Type,
                IdUser = session.IdUser,
                UserName = session.IdUserNavigation?.Name,
                IdPlayer = session.IdPlayer,
                PlayerName = session.IdPlayerNavigation != null ? $"{session.IdPlayerNavigation.Name} {session.IdPlayerNavigation.Surname}" : null,
                Note = session.Note
            };

            return Task.FromResult(Result<GetSessionByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetSessionByIdResponse>.Failure($"An error occurred while retrieving the session: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
