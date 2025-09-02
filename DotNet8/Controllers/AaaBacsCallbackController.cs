using Calendarium.Data;
using Calendarium.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Calendarium.Controllers
{

    public class AaaBacsCallbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AaaBacsCallbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Index([FromBody] dynamic content)
        {
            string unknownJsonString = content.ToString();

            LOG_THIS(_context, unknownJsonString);

            try
            {
                MODEL aaaa = JsonConvert.DeserializeObject<MODEL>(unknownJsonString);
            }
            catch (Exception)
            {
                LOG_THIS(_context, "CANNOT PARSE: " + unknownJsonString);
            }

            return Ok();
        }

        private static void LOG_THIS(ApplicationDbContext context, string textToLog)
        {
            Note note = new Note()
            {
                SortNum = 0,
                Text = DateTime.Now.ToString("MM.dd HH:mm - ") + textToLog
            };
            context.Add(note);
            context.SaveChanges();
        }
    }
}