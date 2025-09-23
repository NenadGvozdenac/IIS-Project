using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Features.MetricTypes.CreateMetricType;

public class CreateMetricTypeHandler : IRequestHandler<CreateMetricTypeCommand, Result<CreateMetricTypeResponse>>
{
    private readonly IMetricTypeRepository _metricTypeRepository;

    public CreateMetricTypeHandler(IMetricTypeRepository metricTypeRepository)
    {
        _metricTypeRepository = metricTypeRepository;
    }

    public Task<Result<CreateMetricTypeResponse>> Handle(CreateMetricTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var metricType = new MetricType
            {
                Type = request.Type
            };

            var createdMetricType = _metricTypeRepository.Create(metricType);

            var response = new CreateMetricTypeResponse
            {
                IdType = createdMetricType.IdType,
                Type = createdMetricType.Type
            };

            return Task.FromResult(Result<CreateMetricTypeResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateMetricTypeResponse>.Failure($"An error occurred while creating metric type: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
