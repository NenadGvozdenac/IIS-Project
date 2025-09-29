using Nest;
using scouting_service.src.Elasticsearch.Models;

namespace scouting_service.src.Elasticsearch.Services
{
    public class SessionSearchService : ISessionSearchService
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<SessionSearchService> _logger;
        private const string IndexName = "sessions";

        public SessionSearchService(IElasticsearchService elasticsearchService, ILogger<SessionSearchService> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task<ISearchResponse<SessionDocument>> SearchSessionsByNotesAsync(string keywords, DateTime? startDate = null, DateTime? endDate = null, int size = 20)
        {
            var searchResponse = await _elasticsearchService.SearchAsync<SessionDocument>(IndexName, new
            {
                query = new
                {
                    @bool = new
                    {
                        must = new object[]
                        {
                            new
                            {
                                multi_match = new
                                {
                                    query = keywords,
                                    fields = new[] { "notes", "description", "tags" },
                                    type = "best_fields",
                                    fuzziness = "AUTO"
                                }
                            }
                        },
                        filter = startDate.HasValue && endDate.HasValue ? new object[]
                        {
                            new
                            {
                                range = new
                                {
                                    startTime = new
                                    {
                                        gte = startDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                                        lte = endDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")
                                    }
                                }
                            }
                        } : null
                    }
                },
                size = size,
                highlight = new
                {
                    fields = new
                    {
                        notes = new { },
                        description = new { }
                    }
                }
            });

            return searchResponse;
        }

        public async Task<ISearchResponse<SessionDocument>> AnalyzeSessionsByTypeAndDateAsync(string sessionType, DateTime startDate, DateTime endDate, int size = 50)
        {
            var searchResponse = await _elasticsearchService.SearchAsync<SessionDocument>(IndexName, new
            {
                query = new
                {
                    @bool = new
                    {
                        must = new object[]
                        {
                            new
                            {
                                term = new
                                {
                                    sessionType = new
                                    {
                                        value = sessionType
                                    }
                                }
                            },
                            new
                            {
                                range = new
                                {
                                    startTime = new
                                    {
                                        gte = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                                        lte = endDate.ToString("yyyy-MM-ddTHH:mm:ssZ")
                                    }
                                }
                            }
                        }
                    }
                },
                size = size,
                aggs = new
                {
                    sessions_over_time = new
                    {
                        date_histogram = new
                        {
                            field = "startTime",
                            calendar_interval = "1M"
                        }
                    },
                    by_user = new
                    {
                        terms = new
                        {
                            field = "user.name.keyword",
                            size = 10
                        }
                    },
                    by_status = new
                    {
                        terms = new
                        {
                            field = "status.keyword",
                            size = 10
                        }
                    },
                    avg_duration = new
                    {
                        avg = new
                        {
                            field = "durationMinutes"
                        }
                    }
                }
            });

            return searchResponse;
        }
    }
}