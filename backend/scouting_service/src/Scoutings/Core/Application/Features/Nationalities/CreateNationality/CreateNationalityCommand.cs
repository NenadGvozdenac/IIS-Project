using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Nationalities.CreateNationality;

public class CreateNationalityCommand : IRequest<Result<CreateNationalityResponse>>
{
    public string? State { get; set; }
}
