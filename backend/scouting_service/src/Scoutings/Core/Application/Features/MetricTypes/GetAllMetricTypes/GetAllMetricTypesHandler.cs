using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.MetricTypes.GetAllMetricTypes;

public class GetAllMetricTypesHandler : IRequestHandler<GetAllMetricTypesQuery, Result<List<GetAllMetricTypesResponse>>>
{
    private readonly IMetricTypeRepository _metricTypeRepository;

    public GetAllMetricTypesHandler(IMetricTypeRepository metricTypeRepository)
    {
        _metricTypeRepository = metricTypeRepository;
    }

    public Task<Result<List<GetAllMetricTypesResponse>>> Handle(GetAllMetricTypesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var metricTypes = _metricTypeRepository.GetAll();

            var response = metricTypes.Select(mt => new GetAllMetricTypesResponse
            {
                IdType = mt.IdType,
                Type = mt.Type
            }).ToList();

            return Task.FromResult(Result<List<GetAllMetricTypesResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllMetricTypesResponse>>.Failure($"An error occurred while retrieving metric types: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
