using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Positions.GetAllPositions;

public class GetAllPositionsHandler : IRequestHandler<GetAllPositionsQuery, Result<List<GetAllPositionsResponse>>>
{
    private readonly IPositionRepository _positionRepository;

    public GetAllPositionsHandler(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public Task<Result<List<GetAllPositionsResponse>>> Handle(GetAllPositionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var positions = _positionRepository.GetAll();

            var response = positions.Select(p => new GetAllPositionsResponse
            {
                IdPosition = p.IdPosition,
                Name = p.Name
            }).ToList();

            return Task.FromResult(Result<List<GetAllPositionsResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllPositionsResponse>>.Failure($"An error occurred while retrieving positions: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
