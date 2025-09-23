using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Reports.GetSectorAnalysisReport;

public class GetSectorAnalysisReportHandler : IRequestHandler<GetSectorAnalysisReportQuery, Result<GetSectorAnalysisReportResponse>>
{
    private readonly IGraphReportsRepository _reportsRepository;

    public GetSectorAnalysisReportHandler(IGraphReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task<Result<GetSectorAnalysisReportResponse>> Handle(GetSectorAnalysisReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var sectorAnalysis = await _reportsRepository.GetSectorAnalysisReport();

            var response = new GetSectorAnalysisReportResponse
            {
                SectorAnalysis = sectorAnalysis
            };

            return Result<GetSectorAnalysisReportResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetSectorAnalysisReportResponse>.Failure($"An error occurred while generating sector analysis report: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}