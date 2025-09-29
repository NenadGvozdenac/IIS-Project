using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Layout.Borders;

namespace scouting_service.src.Scoutings.Core.Application.Features.Reports.GenerateScoutingReport;

public class GenerateScoutingReportHandler : IRequestHandler<GenerateScoutingReportCommand, Result<byte[]>>
{
    private readonly IPlayerRepository _playerRepository;

    public GenerateScoutingReportHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<Result<byte[]>> Handle(GenerateScoutingReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get scouting report data from database
            var reportData = await _playerRepository.GetScoutingReportDataAsync(
                request.SeasonId,
                request.Filters?.Position,
                request.Filters?.Nationality,
                request.Filters?.PlayerName
            );

            if (!reportData.Any())
            {
                return Result<byte[]>.Failure("No players found for the specified criteria");
            }

            // Generate PDF
            var pdfBytes = GeneratePdf(reportData, request);
            
            return Result<byte[]>.Success(pdfBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Detailed error in PDF generation: {ex}");
            return Result<byte[]>.Failure($"Error generating report: {ex.Message}");
        }
    }

    private byte[] GeneratePdf(List<ScoutingReportData> reportData, GenerateScoutingReportCommand request)
    {
        try
        {
            using var stream = new MemoryStream();
            using (var writer = new PdfWriter(stream))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    using (var document = new Document(pdf))
                    {
                        // Add title
                        var title = new Paragraph("Advanced Scouting Report")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(24)
                            .SetBold()
                            .SetMarginBottom(20);
                        document.Add(title);

                        // Add report info
                        var reportInfo = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm}")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(12)
                            .SetMarginBottom(20);
                        document.Add(reportInfo);

                        // Add filters info if any
                        if (request.Filters != null)
                        {
                            var filterInfo = new List<string>();
                            if (!string.IsNullOrEmpty(request.Filters.Position))
                                filterInfo.Add($"Position: {request.Filters.Position}");
                            if (!string.IsNullOrEmpty(request.Filters.Nationality))
                                filterInfo.Add($"Nationality: {request.Filters.Nationality}");
                            if (!string.IsNullOrEmpty(request.Filters.PlayerName))
                                filterInfo.Add($"Player Name: {request.Filters.PlayerName}");

                            if (filterInfo.Any())
                            {
                                var filters = new Paragraph($"Filters Applied: {string.Join(", ", filterInfo)}")
                                    .SetTextAlignment(TextAlignment.CENTER)
                                    .SetFontSize(10)
                                    .SetItalic()
                                    .SetMarginBottom(20);
                                document.Add(filters);
                            }
                        }

                        // Create detailed table
                        var table = new Table(8).UseAllAvailableWidth();
                        
                        // Add headers with styling
                        var headers = new[] { "Rank", "Player", "Position", "Nationality", "Height", "Weight", "Sessions", "Score %" };
                        foreach (var header in headers)
                        {
                            table.AddHeaderCell(new Cell()
                                .Add(new Paragraph(header).SetBold())
                                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetBorder(new SolidBorder(1)));
                        }

                        // Add data rows with alternating colors
                        var rank = 1;
                        foreach (var player in reportData.Take(10)) // Show top 10 players
                        {
                            var backgroundColor = rank % 2 == 0 ? ColorConstants.WHITE : new DeviceRgb(248, 249, 250);
                            
                            table.AddCell(new Cell().Add(new Paragraph(rank.ToString())).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(backgroundColor));
                            table.AddCell(new Cell().Add(new Paragraph(player.PlayerFullName)).SetBackgroundColor(backgroundColor));
                            table.AddCell(new Cell().Add(new Paragraph(player.PositionName)).SetBackgroundColor(backgroundColor));
                            table.AddCell(new Cell().Add(new Paragraph(player.NationalityName)).SetBackgroundColor(backgroundColor));
                            table.AddCell(new Cell().Add(new Paragraph(player.LatestHeight?.ToString() ?? "N/A")).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(backgroundColor));
                            table.AddCell(new Cell().Add(new Paragraph(player.LatestWeight?.ToString() ?? "N/A")).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(backgroundColor));
                            table.AddCell(new Cell().Add(new Paragraph(player.TotalSessionsAnalyzed.ToString())).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(backgroundColor));
                            table.AddCell(new Cell().Add(new Paragraph(player.NormalizedScore.ToString("F1"))).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(backgroundColor));
                            rank++;
                        }

                        document.Add(table);

                        // Add comprehensive summary statistics with smaller gap
                        document.Add(new Paragraph("\nAdvanced Analytics Summary")
                            .SetFontSize(16)
                            .SetBold()
                            .SetMarginTop(10)
                            .SetMarginBottom(8));

                        var totalPlayers = reportData.Count;
                        
                        if (totalPlayers > 0)
                        {
                            var avgScore = reportData.Average(r => r.NormalizedScore);
                            var maxScore = reportData.Max(r => r.NormalizedScore);
                            var minScore = reportData.Min(r => r.NormalizedScore);
                            var topPlayer = reportData.First(r => r.NormalizedScore == maxScore);
                            var bottomPlayer = reportData.First(r => r.NormalizedScore == minScore);
                            var totalSessions = reportData.Sum(r => r.TotalSessionsAnalyzed);
                            var avgSessions = reportData.Average(r => r.TotalSessionsAnalyzed);

                            var stats = new Paragraph()
                                .Add($"Total Players Analyzed: {totalPlayers}\n")
                                .Add($"Average Score: {avgScore:F2}%\n")
                                .Add($"Total Sessions Analyzed: {totalSessions}\n")
                                .Add($"Average Sessions per Player: {avgSessions:F1}")
                                .SetMarginBottom(15);

                            document.Add(stats);

                            // Add top performers section with basketball stats
                            var topPerformers = reportData.Take(5).ToList();
                            if (topPerformers.Any())
                            {
                                document.Add(new Paragraph("Top 5 Performers - Basketball Performance")
                                    .SetFontSize(14)
                                    .SetBold()
                                    .SetMarginBottom(8));

                                // Create a table for top performers with basketball stats
                                var topPerformersTable = new Table(6).UseAllAvailableWidth();
                                
                                // Headers
                                var performerHeaders = new[] { "Rank", "Player", "Score %", "Avg Points", "Avg Assists", "Avg Minutes" };
                                foreach (var header in performerHeaders)
                                {
                                    topPerformersTable.AddHeaderCell(new Cell()
                                        .Add(new Paragraph(header).SetBold())
                                        .SetBackgroundColor(new DeviceRgb(52, 152, 219))
                                        .SetFontColor(ColorConstants.WHITE)
                                        .SetTextAlignment(TextAlignment.CENTER)
                                        .SetBorder(new SolidBorder(1)));
                                }

                                // Add top performer data
                                var performerRank = 1;
                                foreach (var performer in topPerformers)
                                {
                                    var backgroundColor = performerRank % 2 == 0 ? ColorConstants.WHITE : new DeviceRgb(240, 248, 255);
                                    
                                    topPerformersTable.AddCell(new Cell().Add(new Paragraph(performerRank.ToString())).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(backgroundColor));
                                    topPerformersTable.AddCell(new Cell().Add(new Paragraph($"{performer.PlayerFullName} ({performer.PositionName})")).SetBackgroundColor(backgroundColor));
                                    topPerformersTable.AddCell(new Cell().Add(new Paragraph($"{performer.NormalizedScore:F1}%")).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(backgroundColor));
                                    topPerformersTable.AddCell(new Cell().Add(new Paragraph(performer.AveragePoints?.ToString("F1") ?? "N/A")).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(backgroundColor));
                                    topPerformersTable.AddCell(new Cell().Add(new Paragraph(performer.AverageAssists?.ToString("F1") ?? "N/A")).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(backgroundColor));
                                    topPerformersTable.AddCell(new Cell().Add(new Paragraph(performer.AverageMinutes?.ToString("F1") ?? "N/A")).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(backgroundColor));
                                    performerRank++;
                                }
                                
                                document.Add(topPerformersTable);
                                document.Add(new Paragraph("\n"));
                            }
                        }
                        else
                        {
                            var stats = new Paragraph()
                                .Add($"Total Players Analyzed: {totalPlayers}\n")
                                .Add("No player data available for the specified criteria.")
                                .SetMarginBottom(10);

                            document.Add(stats);
                        }

                        // Add footer
                        document.Add(new Paragraph($"\nReport generated by Advanced Scouting System v1.0")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(8)
                            .SetItalic()
                            .SetMarginTop(20));
                    }
                }
            }
            return stream.ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PDF Generation Error: {ex}");
            throw;
        }
    }
}