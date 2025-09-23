using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionTypes.GetAllSessionTypes;

public class GetAllSessionTypesHandler : IRequestHandler<GetAllSessionTypesQuery, Result<GetAllSessionTypesResponse>>
{
    private readonly ISessionTypeRepository _sessionTypeRepository;

    public GetAllSessionTypesHandler(ISessionTypeRepository sessionTypeRepository)
    {
        _sessionTypeRepository = sessionTypeRepository;
    }

    public Task<Result<GetAllSessionTypesResponse>> Handle(GetAllSessionTypesQuery request, CancellationToken cancellationToken)
    {
        var sessionTypes = _sessionTypeRepository.GetAll();

        var sessionTypeDtos = sessionTypes.Select(st => new SessionTypeDto
        {
            Id = st.IdType,
            Name = st.Type ?? string.Empty
        }).ToList();

        var response = new GetAllSessionTypesResponse
        {
            SessionTypes = sessionTypeDtos
        };

        return Task.FromResult(Result<GetAllSessionTypesResponse>.Success(response));
    }
}
