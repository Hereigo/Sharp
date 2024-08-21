using System.Diagnostics;
using DotNet8.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DotNet8.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            var TEST = new CalEventVM()
            {
                Started = DateTime.Now,
                Time = new TimeSpan(0, 0, 0),
                EveryXDays = 0,
                Description = string.Empty,
                Repeat = CalEventRepeat.Once,
                RepeatList = Enum.GetValues(typeof(CalEventRepeat))
                                .Cast<CalEventRepeat>()
                                .Select(e => new SelectListItem
                                {
                                    Value = e.ToString(),
                                    Text = e.ToString()
                                })
                                .ToList()
            };

            return View(TEST);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Privacy(CalEventVM calEventVM) // [Bind("Id,PeriodSize,Period,Status,Modified,Started,Description")] CalEvent calEvent)
        {
            var TEST = calEventVM;

            // TODO:
            // CONTINUE HERE ...

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
