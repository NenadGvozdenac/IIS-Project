using Microsoft.AspNetCore.Mvc;
using elasticsearch_service.src.Services;

namespace elasticsearch_service.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndexController : ControllerBase
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<IndexController> _logger;

        public IndexController(IElasticsearchService elasticsearchService, ILogger<IndexController> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateIndexes()
        {
            try
            {
                var success = await _elasticsearchService.CreateIndexesAsync();
                if (!success)
                    return StatusCode(500, "Failed to create indexes");

                return Ok("Indexes created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating indexes");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteIndexes()
        {
            try
            {
                var success = await _elasticsearchService.DeleteIndexesAsync();
                if (!success)
                    return StatusCode(500, "Failed to delete indexes");

                return Ok("Indexes deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting indexes");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}