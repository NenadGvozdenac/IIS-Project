using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Sessions.CreateSession;

public class CreateSessionCommand : IRequest<Result<CreateSessionResponse>>
{
    public DateOnly StartTime { get; set; }
    public DateOnly? EndTime { get; set; }
    public int IdSessionStatus { get; set; }
    public int IdSessionType { get; set; }
    public int IdUser { get; set; }
    public int IdPlayer { get; set; }
    public string? Note { get; set; }
}
