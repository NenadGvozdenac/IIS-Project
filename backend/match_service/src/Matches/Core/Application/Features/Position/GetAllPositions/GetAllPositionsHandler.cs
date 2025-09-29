using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.Position.GetAllPositions;

public class GetAllPositionsHandler : IRequestHandler<GetAllPositionsQuery, Result<GetAllPositionsResponse>>
{
    private readonly IPositionRepository _positionRepository;

    public GetAllPositionsHandler(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public Task<Result<GetAllPositionsResponse>> Handle(GetAllPositionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var positions = _positionRepository.GetAll();

            var positionResponses = positions.Select(p => new PositionResponse
            {
                IdPosition = p.IdPosition,
                Name = p.Name
            });

            var response = new GetAllPositionsResponse
            {
                Positions = positionResponses
            };

            return Task.FromResult(Result<GetAllPositionsResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetAllPositionsResponse>.Failure($"An error occurred while retrieving positions: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
