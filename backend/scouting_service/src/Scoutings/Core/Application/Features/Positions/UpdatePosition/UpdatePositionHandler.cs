using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Features.Positions.UpdatePosition;

public class UpdatePositionHandler : IRequestHandler<UpdatePositionCommand, Result<UpdatePositionResponse>>
{
    private readonly IPositionRepository _positionRepository;

    public UpdatePositionHandler(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public Task<Result<UpdatePositionResponse>> Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingPosition = _positionRepository.GetById(request.IdPosition);
            if (existingPosition == null)
            {
                return Task.FromResult(Result<UpdatePositionResponse>.Failure("Position not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var position = new Position
            {
                IdPosition = request.IdPosition,
                Name = request.Name
            };

            var updatedPosition = _positionRepository.Update(position);

            var response = new UpdatePositionResponse
            {
                IdPosition = updatedPosition.IdPosition,
                Name = updatedPosition.Name
            };

            return Task.FromResult(Result<UpdatePositionResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdatePositionResponse>.Failure($"An error occurred while updating position: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}