using System.Diagnostics;
using System.Text.Json;
using DotNet8.Data;
using DotNet8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet8.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        private const int _historyLines = 40;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // TODO:

        // use css bundler
        // add TASKS LIST Editable


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string pMonth = "")
        {
            var now = DateTime.UtcNow.AddHours(3);
            var now4currentPage = now;

            if (pMonth == "next")
                now4currentPage = now.AddMonths(1);
            else if (pMonth == "prev")
                now4currentPage = now.AddMonths(-1);

            var events = await _context.CalEvents
                .Where(e => e.Repeat == CalEventRepeat.Monthly
                    || (e.Month == now4currentPage.Month && (e.Year == now4currentPage.Year || e.Repeat == CalEventRepeat.Yearly))
                    || (e.Month <= now4currentPage.Month && e.Year <= now4currentPage.Year && e.Repeat == CalEventRepeat.EveryXdays))
                .ToListAsync();

            await ProcessRequestHeaders(Request.Headers);

            var eventsFullCount = await _context.CalEvents.CountAsync();

            var eventsModel = new List<CalEvent>();
            var monthMaxDay = Utils.Utils.GetMaxDayOfTheMonth(now4currentPage);

            ViewBag.TodayReal = now;
            ViewBag.TodayCurrent = now4currentPage;
            ViewBag.CssChanged = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "/wwwroot/css/site.css"))
                .LastWriteTime.ToString("yyMMddHHmm");
            ViewBag.EnvtsCount = events.Count;
            ViewBag.EnvtsFullCount = eventsFullCount;

            foreach (var evt in events)
            {
                eventsModel.Add(evt);

                if (evt.Repeat == CalEventRepeat.EveryXdays)
                {
                    var nextDate = evt.Started.AddDays(evt.EveryXDays.Value);

                    while (nextDate.Month <= now4currentPage.Month)
                    {
                        eventsModel.Add(new CalEvent()
                        {
                            Id = evt.Id,
                            Day = evt.Started.Day,
                            Description = evt.Description,
                            EveryXDays = evt.EveryXDays,
                            Modified = evt.Modified,
                            Month = (evt.Repeat == CalEventRepeat.Monthly) ? 0 : evt.Started.Month,
                            Repeat = evt.Repeat,
                            Started = nextDate,
                            Status = evt.Status,
                            Time = evt.Time,
                            Year = (evt.Repeat == CalEventRepeat.Yearly) ? 0 : evt.Started.Year
                        });
                        nextDate = nextDate.AddDays(evt.EveryXDays.Value);
                    }
                }
            }

            for (var i = 1; i <= monthMaxDay; i++)
            {
                eventsModel.Add(new CalEvent(new DateTime(now4currentPage.Year, now4currentPage.Month, i)));
            }

            return View(eventsModel);
        }

        // TODO:
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        public IActionResult Create(int? id)
        {
            var now = DateTime.Now;
            var newEvent = new CalEvent()
            {
                EveryXDays = 0,
                Modified = DateTime.Now,
                Started = new DateTime(now.Year, now.Month, id ?? 1),
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
                _context.Add(new CalEvent()
                {
                    Day = evnt.Started.Day,
                    Description = evnt.Description,
                    EveryXDays = evnt.EveryXDays,
                    Modified = DateTime.Now,
                    Month = (evnt.Repeat == CalEventRepeat.Monthly) ? 0 : evnt.Started.Month,
                    Repeat = evnt.Repeat,
                    Started = evnt.Started,
                    Status = CalEventStatus.Active,
                    Time = evnt.Time,
                    Year = (evnt.Repeat == CalEventRepeat.Yearly) ? 0 : evnt.Started.Year,
                });
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
                    calEvent.Day = evnt.Started.Day;
                    calEvent.Description = evnt.Description;
                    calEvent.EveryXDays = evnt.EveryXDays;
                    calEvent.Modified = DateTime.Now;
                    calEvent.Month = (evnt.Repeat == CalEventRepeat.Monthly) ? 0 : evnt.Started.Month;
                    calEvent.Repeat = evnt.Repeat;
                    calEvent.Started = evnt.Started;
                    calEvent.Status = evnt.Status;
                    calEvent.Time = evnt.Time;
                    calEvent.Year = (evnt.Repeat == CalEventRepeat.Yearly) ? 0 : evnt.Started.Year;
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
            if (id == null)
            {
                return NotFound();
            }
            var calEvent = await _context.CalEvents.FirstOrDefaultAsync(m => m.Id == id);
            if (calEvent == null)
            {
                return NotFound();
            }
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

        public async Task<IActionResult> History()
            => View(await _context.RequestsHeaders
                .Where(rh => rh.Field == ReqHeadFieldType.UsrAgent || rh.Field == ReqHeadFieldType.Language)
                .OrderByDescending(rh => rh.Created).Take(_historyLines).ToListAsync());

        public async Task<JsonResult> GetJson()
            => new JsonResult(
                await _context.CalEvents.ToArrayAsync(),
                new JsonSerializerOptions { PropertyNamingPolicy = null });

        [AllowAnonymous]
        public IActionResult Privacy() =>  View();

        [AllowAnonymous]
        public ContentResult StartPage() => base.Content(System.IO.File.ReadAllText("index.html"), "text/html");

        // public async Task<IActionResult> Details(int? id)
        // {
        //     if (id == null) return NotFound();
        //     var calEvent = await _context.CalEvents.FirstOrDefaultAsync(m => m.Id == id);
        //     if (calEvent == null) return NotFound();
        //     return View(calEvent);
        // }

        private bool CalEventExists(int id)
        {
            return _context.CalEvents.Any(e => e.Id == id);
        }

        private async Task ProcessRequestHeaders(IHeaderDictionary headers)
        {
            await ProcessHeader(new RequestHeaderField(ReqHeadFieldType.Accept, headers["Accept"]));
            // await ProcessHeader(new RequestHeaderField(ReqHeadFieldType.Encode, headers["Accept-Encoding"]));
            // await ProcessHeader(new RequestHeaderField(ReqHeadFieldType.Language, headers["Accept-Language"]));
            // await ProcessHeader(new RequestHeaderField(ReqHeadFieldType.Referer, headers["Referer"]));
            await ProcessHeader(new RequestHeaderField(ReqHeadFieldType.UsrAgent, headers["User-Agent"]));
        }

        private async Task ProcessHeader(RequestHeaderField reqHead)
        {
            if (!string.IsNullOrWhiteSpace(reqHead.Text))
            {
                if (!_context.RequestsHeaders.Any(old =>
                    old.Field == reqHead.Field && old.Text == reqHead.Text && old.Created.AddHours(1) > reqHead.Created))
                {
                    _context.RequestsHeaders.Add(reqHead);
                    _context.SaveChanges();
                }
            }
        }
    }
}
