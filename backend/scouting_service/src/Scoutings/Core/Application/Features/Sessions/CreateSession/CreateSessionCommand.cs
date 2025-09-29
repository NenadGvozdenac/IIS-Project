using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Sessions.CreateSession;

public class CreateSessionCommand : IRequest<Result<CreateSessionResponse>>
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int IdSessionStatus { get; set; }
    public int IdSessionType { get; set; }
    public int IdUser { get; set; }
    public int IdPlayer { get; set; }
}
