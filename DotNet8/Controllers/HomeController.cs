using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Calendarium.Data;
using Calendarium.Models;
using Calendarium.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Calendarium.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<HomeController> _logger;
        private readonly CalendarService _calendar;
        private readonly RequestHeaderLogger _headerLogger;

        private const int _historyLines = 40;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, IHostEnvironment hostEnvironment,
            CalendarService calendar, RequestHeaderLogger headerLogger)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _calendar = calendar;
            _headerLogger = headerLogger;

            ViewBag.CssChanged =
                new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "/wwwroot/css/site.min.css")).LastWriteTime.ToString("yyMMddHHmm");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        [AllowAnonymous]
        public IActionResult Privacy() => View();

        [AllowAnonymous]
        public ContentResult StartPage() => base.Content(System.IO.File.ReadAllText("index.html"), "text/html");

        [AllowAnonymous]
        public async Task<IActionResult> Index(string pMonth = "")
        {
            await _headerLogger.LogAsync(Request.Headers);

            DateTime today = _calendar.Today();
            var viewMonth = pMonth switch
            {
                "next" => today.AddMonths(1),
                "prev" => today.AddMonths(-1),
                _ => today,
            };

            var sheet = await _calendar.BuildSheetAsync(viewMonth.Year, viewMonth.Month);
            var eventsModel = sheet.Occurrences;

            ViewBag.EnvtsCount = eventsModel.Count;

            // Fill empty days:
            for (var i = 0; i < CalendarService.SheetDays; i++) { eventsModel.Add(new CalEvent(sheet.FirstDay.AddDays(i))); }

            ViewBag.EnvtsFullCount = sheet.TotalCount;
            ViewBag.IsDevEnv = _hostEnvironment.IsDevelopment();
            ViewBag.TodayCurrent = viewMonth;
            ViewBag.TodayReal = today;
            ViewBag.SheetFirstDay = sheet.FirstDay;

            ViewBag.IsCalendarPage = true;

            return View(eventsModel);
        }

        // TODO:
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        public async Task<IActionResult> FullList()
        {
            var events = await _context.CalEvents.OrderBy(x => x.Month).ThenBy(x => x.Day).ToListAsync();
            return View(events);
        }

        public IActionResult Create(int? id, int? month)
        {
            var now = DateTime.Now;
            var newEvent = new CalEvent()
            {
                EveryXDays = 0,
                Modified = DateTime.Now,
                Started = new DateTime(now.Year, month ?? now.Month, id ?? 1),
                Status = CalEventStatus.Active,
                Time = new TimeSpan(0, 0, 0),
                Repeat = CalEventRepeat.Once,
            };
            return View(newEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CalEvent evnt)
        {
            if (evnt != null && ModelState.IsValid)
            {
                var newEvent = new CalEvent()
                {
                    Created = DateTime.Now,
                    Description = evnt.Description,
                    Modified = DateTime.Now,
                    Status = CalEventStatus.Active,
                    Time = evnt.Time,
                };
                CalendarService.ApplySchedule(newEvent, evnt.Started, evnt.Repeat, evnt.EveryXDays);
                _context.Add(newEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(evnt);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var calEvent = await _context.CalEvents.FindAsync(id);
            if (calEvent == null)
            {
                return NotFound();
            }
            return View(calEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CalEvent evnt)
        {
            if (id != evnt.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var calEvent = await _context.CalEvents.FindAsync(id);
                    if (calEvent == null)
                    {
                        return NotFound();
                    }
                    calEvent.Description = evnt.Description;
                    calEvent.Modified = DateTime.Now;
                    calEvent.Status = evnt.Status;
                    calEvent.Time = evnt.Time;
                    CalendarService.ApplySchedule(calEvent, evnt.Started, evnt.Repeat, evnt.EveryXDays);
                    _context.Update(calEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalEventExists(evnt.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(evnt);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var calEvent = await _context.CalEvents.FirstOrDefaultAsync(m => m.Id == id);
            if (calEvent == null) return NotFound();
            return View(calEvent);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calEvent = await _context.CalEvents.FindAsync(id);
            if (calEvent != null)
            {
                _context.CalEvents.Remove(calEvent);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> History() => View(await _context.RequestsHeaders
            .Where(rh => rh.Field == ReqHeadFieldType.UsrAgent || rh.Field == ReqHeadFieldType.Language)
            .OrderByDescending(rh => rh.Created).Take(_historyLines).ToListAsync());

        public async Task<JsonResult> GetJson() => new JsonResult(
            await _context.CalEvents.ToArrayAsync(),
            new JsonSerializerOptions { PropertyNamingPolicy = null });

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UploadFile(IFormFile uploadingFile)
        {
            if (_hostEnvironment.IsDevelopment() && uploadingFile != null)
            {
                if (uploadingFile.Length < 2097152) // 2 MB
                {
                    try
                    {
                        var result = new StringBuilder();
                        using (var reader = new StreamReader(uploadingFile.OpenReadStream()))
                        {
                            while (reader.Peek() >= 0)
                                result.AppendLine(reader.ReadLine());
                        }
                        List<CalEvent>? CalEventsList = JsonSerializer.Deserialize<List<CalEvent>>(result.ToString());

                        if (CalEventsList != null)
                        {
                            foreach (var evt in CalEventsList)
                            {
                                if (evt != null)
                                {
                                    var imported = new CalEvent()
                                    {
                                        Created = DateTime.Now,
                                        Description = evt.Description,
                                        Modified = DateTime.Now,
                                        Status = CalEventStatus.Active,
                                        Time = evt.Time,
                                    };
                                    CalendarService.ApplySchedule(imported, evt.Started, evt.Repeat, evt.EveryXDays);
                                    _context.CalEvents.Add(imported);
                                }
                            }
                            await _context.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        // TODO:
                        // smth...
                    }
                }
            }
            return RedirectToAction("Index");
        }

        private bool CalEventExists(int id)
        {
            return _context.CalEvents.Any(e => e.Id == id);
        }
    }
}
