using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Zones.DeleteZone;

public class DeleteZoneHandler : IRequestHandler<DeleteZoneCommand, Result<DeleteZoneResponse>>
{
    private readonly IZoneRepository _zoneRepository;

    public DeleteZoneHandler(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public Task<Result<DeleteZoneResponse>> Handle(DeleteZoneCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var zone = _zoneRepository.GetById(request.Id);
            if (zone == null)
            {
                return Task.FromResult(Result<DeleteZoneResponse>.Failure($"Zone with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var isDeleted = _zoneRepository.Delete(request.Id);

            if (!isDeleted)
            {
                return Task.FromResult(Result<DeleteZoneResponse>.Failure($"Failed to delete zone with ID {request.Id}")
                    .WithCode((int)ResultCode.InternalServerError));
            }

            var response = new DeleteZoneResponse
            {
                Success = true,
                Message = $"Zone with ID {request.Id} has been successfully deleted"
            };

            return Task.FromResult(Result<DeleteZoneResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<DeleteZoneResponse>.Failure($"An error occurred while deleting the zone: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
