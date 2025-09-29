using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Text;

namespace travel_service.src.Travels.Core.Application.Features.Trip.GetTravelCostReport;

public class GetTravelCostReportHandler : IRequestHandler<GetTravelCostReportQuery, Result<TravelCostReportDto>>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetTravelCostReportHandler> _logger;

    public GetTravelCostReportHandler(IConfiguration configuration, ILogger<GetTravelCostReportHandler> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<TravelCostReportDto>> Handle(GetTravelCostReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Generating travel cost report");

            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            // Pozivamo PL/SQL funkciju za generisanje izveštaja troškova putovanja sa explicit casting
            var sql = @"SELECT 
                        ((report).match_details)::text as match_details,
                        ((report).cost_summary)::text as cost_summary,
                        ((report).team_members_by_match)::text as team_members,
                        ((report).management_members_by_match)::text as management_members
                      FROM (SELECT generate_travel_cost_report() as report) t";
            
            _logger.LogDebug("Executing SQL: {Sql}", sql);

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                _logger.LogWarning("No travel cost report data found");
                return Result<TravelCostReportDto>.Failure("No travel cost data available");
            }

            _logger.LogDebug("Successfully read data from database");
            _logger.LogDebug("Field count: {FieldCount}", reader.FieldCount);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                _logger.LogDebug("Field {Index}: {Name} = {Value}", i, reader.GetName(i), reader.IsDBNull(i) ? "NULL" : reader.GetValue(i)?.ToString() ?? "NULL");
            }

            var result = new TravelCostReportDto();

            // Parsiranje match_details array iz PostgreSQL-a
            var matchDetailsArray = reader.IsDBNull(0) ? null : reader["match_details"] as string;
            var teamMembersArray = reader.IsDBNull(2) ? null : reader["team_members"] as string;
            var managementMembersArray = reader.IsDBNull(3) ? null : reader["management_members"] as string;
            
            _logger.LogDebug("Raw match_details from database: {MatchDetails}", matchDetailsArray);
            _logger.LogDebug("Raw team_members from database: {TeamMembers}", teamMembersArray);
            _logger.LogDebug("Raw management_members from database: {ManagementMembers}", managementMembersArray);
            
            if (!string.IsNullOrEmpty(matchDetailsArray))
            {
                _logger.LogDebug("Processing non-empty match details array");
                var matchCosts = ParseMatchCostsFromArray(matchDetailsArray, teamMembersArray, managementMembersArray);
                _logger.LogDebug("Parsed {Count} match costs", matchCosts.Count);
                
                result.IndividualCosts = matchCosts;
                result.TotalCosts = CalculateTotalCosts(matchCosts);
            }
            else
            {
                result.IndividualCosts = new List<MatchCostDto>();
                result.TotalCosts = new TotalCostsDto 
                { 
                    Transportation = 0, 
                    Accommodation = 0, 
                    Total = 0, 
                    MatchCount = 0 
                };
            }

            _logger.LogInformation("Successfully generated travel cost report with {MatchCount} matches", result.TotalCosts.MatchCount);

            return Result<TravelCostReportDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating travel cost report");
            return Result<TravelCostReportDto>.Failure($"Error generating travel cost report: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }

    private List<MatchCostDto> ParseMatchCostsFromArray(string arrayString, string? teamMembersArray = null, string? managementMembersArray = null)
    {
        var matchCosts = new List<MatchCostDto>();
        
        try
        {
            _logger.LogDebug("Starting to parse array string: {ArrayString}", arrayString);
            
            if (string.IsNullOrEmpty(arrayString) || arrayString == "{}" || arrayString == "null")
            {
                _logger.LogDebug("Array string is empty or null");
                return matchCosts;
            }

            // Uklanjamo spoljašnje zagrade
            var cleanArray = arrayString.Trim('{', '}');
            _logger.LogDebug("Clean array after trimming: {CleanArray}", cleanArray);
            
            var tuples = ParsePostgreSQLArray(cleanArray);
            _logger.LogDebug("Parsed {TupleCount} tuples from array", tuples.Count);
            
            // Parse team and management members arrays
            var teamMembersByMatch = ParseMembersArray(teamMembersArray);
            var managementMembersByMatch = ParseMembersArray(managementMembersArray);
            
            for (int i = 0; i < tuples.Count; i++)
            {
                var tuple = tuples[i];
                _logger.LogDebug("Processing tuple {Index}: {Tuple}", i, tuple);
                
                // Get corresponding team and management members for this match
                var teamMembers = i < teamMembersByMatch.Count ? teamMembersByMatch[i] : new List<string>();
                var managementMembers = i < managementMembersByMatch.Count ? managementMembersByMatch[i] : new List<string>();
                
                var matchCost = ParseMatchCostFromTuple(tuple, teamMembers, managementMembers);
                if (matchCost != null)
                {
                    _logger.LogDebug("Successfully parsed match cost for match: {MatchName}", matchCost.Match.Name);
                    matchCosts.Add(matchCost);
                }
                else
                {
                    _logger.LogWarning("Failed to parse tuple: {Tuple}", tuple);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing match costs array: {Array}", arrayString);
        }
        
        _logger.LogDebug("Returning {Count} parsed match costs", matchCosts.Count);
        return matchCosts;
    }

    private List<string> ParsePostgreSQLArray(string arrayString)
    {
        var tuples = new List<string>();
        
        try
        {
            _logger.LogDebug("Parsing PostgreSQL array: {ArrayString}", arrayString);
            
            // PostgreSQL array format: "tuple1","tuple2","tuple3"
            // Need to split by quotes and commas while respecting quoted content
            
            var current = new StringBuilder();
            var inQuotes = false;
            var escaped = false;
            
            for (int i = 0; i < arrayString.Length; i++)
            {
                var c = arrayString[i];
                
                if (escaped)
                {
                    current.Append(c);
                    escaped = false;
                    continue;
                }
                
                if (c == '\\')
                {
                    escaped = true;
                    current.Append(c);
                    continue;
                }
                
                if (c == '"')
                {
                    if (inQuotes)
                    {
                        // End of quoted string - add to tuples
                        var tupleContent = current.ToString();
                        if (!string.IsNullOrEmpty(tupleContent))
                        {
                            tuples.Add(tupleContent);
                            _logger.LogDebug("Found tuple: {Tuple}", tupleContent);
                        }
                        current.Clear();
                        inQuotes = false;
                    }
                    else
                    {
                        // Start of quoted string
                        inQuotes = true;
                    }
                }
                else if (inQuotes)
                {
                    current.Append(c);
                }
                // Skip commas and spaces outside quotes
            }
            
            _logger.LogDebug("Parsed {Count} tuples total", tuples.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing PostgreSQL array");
        }
        
        return tuples;
    }

    private List<List<string>> ParseMembersArray(string? arrayString)
    {
        var membersByMatch = new List<List<string>>();
        
        try
        {
            if (string.IsNullOrEmpty(arrayString) || arrayString == "{}" || arrayString == "null")
            {
                return membersByMatch;
            }
            
            // PostgreSQL array format: {"string1","string2","string3"}
            var cleanArray = arrayString.Trim('{', '}');
            var memberStrings = ParsePostgreSQLArray(cleanArray);
            
            foreach (var memberString in memberStrings)
            {
                // Each string contains: "Match X (MatchName): Member1, Member2, Member3"
                var members = new List<string>();
                var colonIndex = memberString.IndexOf(": ");
                
                if (colonIndex >= 0 && colonIndex < memberString.Length - 2)
                {
                    var membersText = memberString.Substring(colonIndex + 2);
                    if (!string.IsNullOrEmpty(membersText) && membersText != "No team members" && membersText != "No management members")
                    {
                        members = membersText.Split(',').Select(m => m.Trim()).Where(m => !string.IsNullOrEmpty(m)).ToList();
                    }
                }
                
                membersByMatch.Add(members);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing members array: {Array}", arrayString);
        }
        
        return membersByMatch;
    }

    private MatchCostDto? ParseMatchCostFromTuple(string tuple, List<string> teamMembers, List<string> managementMembers)
    {
        try
        {
            _logger.LogDebug("Starting to parse tuple: {Tuple}", tuple);
            
            // Uklanjamo zagrade i parsiramo tuple
            var content = tuple.Trim('(', ')');
            var fields = SplitTupleFields(content);
            
            _logger.LogDebug("Split tuple into {FieldCount} fields: {Fields}", fields.Count, string.Join(" | ", fields));
            
            if (fields.Count < 18) 
            {
                _logger.LogWarning("Tuple has only {FieldCount} fields, expected at least 18", fields.Count);
                return null;
            }
            
            var matchInfo = new MatchInfoDto
            {
                Name = CleanField(fields[1]) ?? "Unknown Match",                    // match_name
                Date = SafeParseDateTime(CleanField(fields[2])),                    // match_date
                City = CleanField(fields[4]) ?? "Unknown City",                     // city
                Hall = CleanField(fields[5]) ?? "Unknown Hall",                     // hall
                OpponentTeam = CleanField(fields[3]) ?? "Unknown Team"                     // opponent_team
            };
            
            var transportationCost = SafeParseDecimal(CleanField(fields[6]));       // transportation_cost
            var accommodationCost = SafeParseDecimal(CleanField(fields[10]));       // accommodation_cost
            
            var transportationDetails = new OfferDetailsDto
            {
                Company = CleanField(fields[9]),                                     // transportation_company
                Type = CleanField(fields[8]),                                        // transportation_type
                Name = CleanField(fields[7])                                         // transportation_agency
            };
            
            var accommodationDetails = new OfferDetailsDto
            {
                Name = CleanField(fields[12]),                                       // accommodation_name
                Type = CleanField(fields[13]),                                       // accommodation_type
                Company = CleanField(fields[11])                                     // accommodation_agency
            };
            
            // Stvarni podaci o putnicima iz PL/SQL funkcije
            var travelers = new TravelersDto
            {
                TeamMembers = teamMembers,
                Management = managementMembers
            };
            
            var result = new MatchCostDto
            {
                Match = matchInfo,
                Transportation = new CostDetailsDto
                {
                    Cost = transportationCost,
                    Details = !string.IsNullOrEmpty(transportationDetails.Company) ? transportationDetails : null
                },
                Accommodation = new CostDetailsDto
                {
                    Cost = accommodationCost,
                    Details = !string.IsNullOrEmpty(accommodationDetails.Name) ? accommodationDetails : null
                },
                TotalCost = transportationCost + accommodationCost,
                Travelers = travelers
            };
            
            _logger.LogDebug("Successfully parsed match: {MatchName} with total cost: {TotalCost}", 
                matchInfo.Name, result.TotalCost);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing tuple: {Tuple}", tuple);
            return null;
        }
    }

    private List<string> SplitTupleFields(string content)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;
        var escaped = false;
        
        for (int i = 0; i < content.Length; i++)
        {
            var c = content[i];
            
            if (escaped)
            {
                current.Append(c);
                escaped = false;
                continue;
            }
            
            if (c == '\\')
            {
                escaped = true;
                current.Append(c);
                continue;
            }
            
            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue; // Don't include quotes in the field
            }
            
            if (!inQuotes && c == ',')
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }
        
        // Add the last field
        fields.Add(current.ToString());
        
        return fields;
    }

    private string? CleanField(string? field)
    {
        if (string.IsNullOrEmpty(field) || field.Trim() == "" || field == "null")
            return null;
        
        // Uklanjamo sve tipove quotes i escape karaktera
        var cleaned = field.Trim();
        
        // Uklanjamo escape sequences
        cleaned = cleaned.Replace("\\\"", "\"");
        cleaned = cleaned.Replace("\\'", "'");
        
        // Uklanjamo početne i završne quotes
        if (cleaned.StartsWith("\"") && cleaned.EndsWith("\""))
            cleaned = cleaned.Substring(1, cleaned.Length - 2);
        
        if (cleaned.StartsWith("'") && cleaned.EndsWith("'"))
            cleaned = cleaned.Substring(1, cleaned.Length - 2);
        
        // Još jednom čistimo space-ove
        cleaned = cleaned.Trim();
        
        return string.IsNullOrEmpty(cleaned) ? null : cleaned;
    }

    private decimal SafeParseDecimal(string? value)
    {
        if (string.IsNullOrEmpty(value) || value == "null")
            return 0;
        return decimal.TryParse(value, out var result) ? result : 0;
    }

    private int SafeParseInt(string? value)
    {
        if (string.IsNullOrEmpty(value) || value == "null")
            return 0;
        return int.TryParse(value, out var result) ? result : 0;
    }

    private DateTime SafeParseDateTime(string? value)
    {
        _logger.LogInformation($"SafeParseDateTime called with value: '{value}'");
        
        if (string.IsNullOrEmpty(value) || value == "null")
        {
            _logger.LogWarning("Date value is null or empty, returning MinValue");
            return DateTime.MinValue;
        }

        // Try parsing with various formats
        var formats = new[]
        {
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd",
            "MM/dd/yyyy",
            "dd.MM.yyyy",
            "dd/MM/yyyy HH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss"
        };

        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(value, format, null, System.Globalization.DateTimeStyles.None, out var exactResult))
            {
                _logger.LogInformation($"Successfully parsed date '{value}' using format '{format}' -> {exactResult}");
                return exactResult;
            }
        }

        if (DateTime.TryParse(value, out var result))
        {
            _logger.LogInformation($"Successfully parsed date '{value}' using default parsing -> {result}");
            return result;
        }

        _logger.LogWarning($"Failed to parse date '{value}', returning MinValue");
        return DateTime.MinValue;
    }

    private TotalCostsDto CalculateTotalCosts(List<MatchCostDto> matchCosts)
    {
        return new TotalCostsDto
        {
            Transportation = matchCosts.Sum(m => m.Transportation.Cost),
            Accommodation = matchCosts.Sum(m => m.Accommodation.Cost),
            Total = matchCosts.Sum(m => m.TotalCost),
            MatchCount = matchCosts.Count
        };
    }
}