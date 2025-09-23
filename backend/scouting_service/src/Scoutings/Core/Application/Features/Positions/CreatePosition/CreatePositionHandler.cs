using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Features.Positions.CreatePosition;

public class CreatePositionHandler : IRequestHandler<CreatePositionCommand, Result<CreatePositionResponse>>
{
    private readonly IPositionRepository _positionRepository;

    public CreatePositionHandler(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public Task<Result<CreatePositionResponse>> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var position = new Position
            {
                Name = request.Name
            };

            var createdPosition = _positionRepository.Create(position);

            var response = new CreatePositionResponse
            {
                IdPosition = createdPosition.IdPosition,
                Name = createdPosition.Name
            };

            return Task.FromResult(Result<CreatePositionResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreatePositionResponse>.Failure($"An error occurred while creating position: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
