using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Calendarium.Controllers
{
    [ApiController]
    [Route("api/bacs")]
    public class BacsCallbackController : ControllerBase
    {
        private const string _apiKeyHeader = "X-API-Key";

        private readonly ILogger<BacsCallbackController> _logger;

        public BacsCallbackController(ILogger<BacsCallbackController> logger)
        {
            _logger = logger;
        }

        [HttpPost("callback")]
        public IActionResult ProcessCallback([FromBody] JsonElement content)
        {
            string? apiKey = Request.Headers[_apiKeyHeader];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogWarning("BACS callback rejected: missing {Header} header.", _apiKeyHeader);
                return Unauthorized();
            }

            // TODO: validate the API key and process the callback.
            _logger.LogInformation("BACS callback processed.");
            return Ok();
        }
    }
}
