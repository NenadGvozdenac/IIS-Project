using Nest;
using scouting_service.src.Elasticsearch.Models;

namespace scouting_service.src.Elasticsearch.Services
{
    public class PlayerSearchService : IPlayerSearchService
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<PlayerSearchService> _logger;
        private const string IndexName = "players";

        public PlayerSearchService(IElasticsearchService elasticsearchService, ILogger<PlayerSearchService> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task<ISearchResponse<PlayerDocument>> SearchPlayersAsync(string searchTerm, int size = 20)
        {
            var searchResponse = await _elasticsearchService.SearchAsync<PlayerDocument>(IndexName, new
            {
                query = new
                {
                    multi_match = new
                    {
                        query = searchTerm,
                        fields = new[] { "name", "surname", "position.name", "nationality.state" },
                        type = "best_fields",
                        fuzziness = "AUTO"
                    }
                },
                size = size
            });

            return searchResponse;
        }

        public async Task<ISearchResponse<PlayerDocument>> AnalyzePlayerPerformanceByPositionAsync(string position, int size = 20)
        {
            var searchResponse = await _elasticsearchService.SearchAsync<PlayerDocument>(IndexName, new
            {
                query = new
                {
                    term = new
                    {
                        position = new
                        {
                            name = new
                            {
                                value = position
                            }
                        }
                    }
                },
                size = size,
                sort = new object[]
                {
                    new
                    {
                        physicalMetrics = new
                        {
                            verticalJump = new
                            {
                                order = "desc",
                                nested = new
                                {
                                    path = "physicalMetrics"
                                }
                            }
                        }
                    }
                },
                aggs = new
                {
                    avg_height = new
                    {
                        avg = new
                        {
                            field = "height"
                        }
                    },
                    avg_weight = new
                    {
                        avg = new
                        {
                            field = "weight"
                        }
                    },
                    avg_vertical_jump = new
                    {
                        nested = new
                        {
                            path = "physicalMetrics"
                        },
                        aggs = new
                        {
                            avg_jump = new
                            {
                                avg = new
                                {
                                    field = "physicalMetrics.verticalJump"
                                }
                            }
                        }
                    }
                }
            });

            return searchResponse;
        }

        public async Task<ISearchResponse<PlayerDocument>> ComparePhysicalMetricsAsync(PhysicalMetricsFilter filter)
        {
            var must = new List<object>();

            if (filter.MinVerticalJump.HasValue)
            {
                must.Add(new
                {
                    nested = new
                    {
                        path = "physicalMetrics",
                        query = new
                        {
                            range = new
                            {
                                physicalMetrics = new
                                {
                                    verticalJump = new
                                    {
                                        gte = filter.MinVerticalJump.Value
                                    }
                                }
                            }
                        }
                    }
                });
            }

            if (filter.MaxFatPercentage.HasValue)
            {
                must.Add(new
                {
                    nested = new
                    {
                        path = "physicalMetrics",
                        query = new
                        {
                            range = new
                            {
                                physicalMetrics = new
                                {
                                    fatPercentage = new
                                    {
                                        lte = filter.MaxFatPercentage.Value
                                    }
                                }
                            }
                        }
                    }
                });
            }

            if (filter.MinWingspan.HasValue)
            {
                must.Add(new
                {
                    nested = new
                    {
                        path = "physicalMetrics",
                        query = new
                        {
                            range = new
                            {
                                physicalMetrics = new
                                {
                                    wingspan = new
                                    {
                                        gte = filter.MinWingspan.Value
                                    }
                                }
                            }
                        }
                    }
                });
            }

            var searchResponse = await _elasticsearchService.SearchAsync<PlayerDocument>(IndexName, new
            {
                query = new
                {
                    @bool = new
                    {
                        must = must.ToArray()
                    }
                },
                size = filter.Size,
                aggs = new
                {
                    min_bench_press = new
                    {
                        nested = new
                        {
                            path = "physicalMetrics"
                        },
                        aggs = new
                        {
                            min_bench = new
                            {
                                min = new
                                {
                                    field = "physicalMetrics.benchPressWeight"
                                }
                            }
                        }
                    },
                    max_bench_press = new
                    {
                        nested = new
                        {
                            path = "physicalMetrics"
                        },
                        aggs = new
                        {
                            max_bench = new
                            {
                                max = new
                                {
                                    field = "physicalMetrics.benchPressWeight"
                                }
                            }
                        }
                    },
                    min_squat = new
                    {
                        nested = new
                        {
                            path = "physicalMetrics"
                        },
                        aggs = new
                        {
                            min_squat_val = new
                            {
                                min = new
                                {
                                    field = "physicalMetrics.squatWeight"
                                }
                            }
                        }
                    },
                    max_squat = new
                    {
                        nested = new
                        {
                            path = "physicalMetrics"
                        },
                        aggs = new
                        {
                            max_squat_val = new
                            {
                                max = new
                                {
                                    field = "physicalMetrics.squatWeight"
                                }
                            }
                        }
                    }
                }
            });

            return searchResponse;
        }
    }
}