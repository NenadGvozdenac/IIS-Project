using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Reports.GenerateScoutingReport;

public class GenerateScoutingReportCommand : IRequest<Result<byte[]>>
{
    public int? SeasonId { get; set; } = null; // Make nullable for general summaries across all seasons
    public ReportFilters? Filters { get; set; }
}

public class ReportFilters
{
    public string? Position { get; set; }
    public string? Nationality { get; set; }
    public string? PlayerName { get; set; }
}