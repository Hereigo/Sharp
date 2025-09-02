using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Calendarium.Controllers
{
    [ApiController]
    [Route("api/bacs")]
    public class BacsCallbackController : ControllerBase
    {
        // private readonly CommonLib.Logging.Core.ILogger _logger;
        // private readonly IBacsCallbackService _bacsCallbackService;

        public BacsCallbackController
        (
            // CommonLib.Logging.Core.ILogger logger,
            // IBacsCallbackService bacsCallbackService
        )
        {
            // _logger = logger;
            // _bacsCallbackService = bacsCallbackService;
        }

        private IActionResult ReturnBadRequestAndLog(string message)
        {
            // _logger.Log("Bad Request: {0}", message);
            return BadRequest(message);
        }

        [HttpPost("callback")]
        public IActionResult ProcessCallback([FromBody] dynamic content)
        {
            string apiKey = string.Empty;
            try
            {
                apiKey = GetApiKey();
                // _bacsCallbackService.Process(apiKey, string.Empty);
            }
            catch (Exception)  // AccessDeniedException) // TEST !!!!!!!!!!!!!!!!
            {
                //_logger.Log($"Access denied for apiKey {apiKey}");
                return Challenge();
            }
            // catch (Exception e) // TEST !!!!!!!!!!!!!!!!
            // {
            //     _logger.Log(e.ToString());
            //     return ReturnBadRequestAndLog("Error during execution");
            // }

            //_logger.Log($"Processing OK");

            return Ok();
        }

        #region Helpers
        private string GetApiKey()
        {
            StringValues headerValue;            
            if (!Request.Headers.TryGetValue("X-API-Key", out headerValue))
            {
                //throw new AccessDeniedException();
            }

            return headerValue.FirstOrDefault();
        }
        #endregion
    }
}
