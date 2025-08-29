using Calendarium.Data;
using Calendarium.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Calendarium.Controllers
{
    public class WebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WebhookController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Index([FromBody] dynamic content)
        {
            string undefinedJson = DateTime.Now.ToString("MM.dd_HH:mm") + " - " + content.ToString();

            if (string.IsNullOrWhiteSpace(undefinedJson))
            {
                return BadRequest();
            }

            Note note = new Note()
            {
                SortNum = 0,
                Text = undefinedJson
            };

            _context.Add(note);

            _context.SaveChanges();

            return Ok();
        }
    }
}
// {
//   "SubmissionId": "442329a5-83c4-4696-831b-312492982b81",
//   "Success": true,
//   "NextAction": "None",
//   "CreationDate": "2025-08-29T16:56:27.8077314Z",
//   "Message": "rejected"
// }