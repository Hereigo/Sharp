using System.Diagnostics;
using System.Text;
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
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<HomeController> _logger;

        private const int _historyLines = 40;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, IHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _logger = logger;

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
            await ProcessRequestHeaders(Request.Headers);

            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            var todayForCurrentView = today;

            if (pMonth == "next") todayForCurrentView = today.AddMonths(1);
            else if (pMonth == "prev") todayForCurrentView = today.AddMonths(-1);

            DateTime currMonStart = new DateTime(todayForCurrentView.Year, todayForCurrentView.Month, 1);

            int monBeginDayOfWeek = (int)currMonStart.DayOfWeek; // Mon = 1 \ .. \ Sat = 6 \ Sun = 0

            int sheetSize = 7 * 6;
            int prevMonDaysOnSheet = monBeginDayOfWeek == 0 ? 6 : monBeginDayOfWeek - 1;
            int currMonDays = new DateTime(todayForCurrentView.Year, todayForCurrentView.Month, 1).AddMonths(1).AddDays(-1).Day;
            int nextMonDaysOnSheet = sheetSize - (currMonDays + prevMonDaysOnSheet);

            DateTime sheetFirstDay = monBeginDayOfWeek == 1 ? currMonStart : currMonStart.AddDays(-prevMonDaysOnSheet);
            DateTime sheetLastDay = new DateTime(todayForCurrentView.Year, todayForCurrentView.Month, nextMonDaysOnSheet).AddMonths(1);

            // Gat All already started Events with applicable non-repeating:
            var events = await _context.CalEvents
                .Where(e => e.Started <= sheetLastDay && (e.Repeat != CalEventRepeat.Once || e.Repeat == CalEventRepeat.Once && e.Started >= sheetFirstDay))
                .ToListAsync();

            var allEventsCount = await _context.CalEvents.CountAsync();

            var eventsModel = GenerateRepeatingEvents(events, sheetFirstDay, sheetLastDay, todayForCurrentView);

            ViewBag.EnvtsCount = eventsModel.Count;

            // Fill empty days:
            for (var i = 0; i < sheetSize; i++) eventsModel.Add(new CalEvent(sheetFirstDay.AddDays(i)));

            ViewBag.EnvtsFullCount = allEventsCount;
            ViewBag.IsDevEnv = _hostEnvironment.IsDevelopment();
            ViewBag.TodayCurrent = todayForCurrentView;
            ViewBag.TodayReal = today;
            ViewBag.SheetFirstDay = sheetFirstDay;

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
        public async Task<IActionResult> UploadFile(IFormFile uploadingFile)
        {
            if (uploadingFile != null)
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
                        var CalEventsList = JsonSerializer.Deserialize<List<CalEvent>>(result.ToString());

                        _context.AddRangeAsync(CalEventsList);

                        await _context.SaveChangesAsync();
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

        private List<CalEvent> GenerateRepeatingEvents(List<CalEvent> events, DateTime sheetFirstDay, DateTime sheetLastDay, DateTime today)
        {
            var eventsModel = new List<CalEvent>();
            int currMonMaxDay = Utils.Utils.GetMaxDayOfTheMonth(today);

            foreach (var evt in events)
            {
                if (evt.Repeat == CalEventRepeat.Monthly)
                {
                    for (int i = -1; i <= 1; i++) // Add for 3 month (Prev, Current, Next and Prev.):
                    {
                        var startedDateForMonthly = new DateTime(today.Year, today.Month, evt.Day).AddMonths(i); // 21,03,2025 ??? MONTHLY

                        if (startedDateForMonthly >= sheetFirstDay && startedDateForMonthly <= sheetLastDay)
                        {
                            eventsModel.Add(new CalEvent()
                            {
                                Id = evt.Id,
                                Day = evt.Started.Day,
                                Description = evt.Description,
                                EveryXDays = evt.EveryXDays,
                                Modified = evt.Modified,
                                Month = startedDateForMonthly.Month,
                                Repeat = evt.Repeat,
                                Started = startedDateForMonthly,
                                Status = evt.Status,
                                Time = evt.Time,
                                Year = startedDateForMonthly.Year
                            });
                        }
                    }
                }
                else if (evt.Repeat == CalEventRepeat.Yearly)
                {
                    var startedDateForYearly = new DateTime(today.Year, evt.Month, evt.Day);

                    if (startedDateForYearly >= sheetFirstDay && startedDateForYearly <= sheetLastDay)
                    {
                        eventsModel.Add(new CalEvent()
                        {
                            Id = evt.Id,
                            Day = evt.Started.Day,
                            Description = evt.Description,
                            EveryXDays = evt.EveryXDays,
                            Modified = evt.Modified,
                            Month = startedDateForYearly.Month,
                            Repeat = evt.Repeat,
                            Started = startedDateForYearly,
                            Status = evt.Status,
                            Time = evt.Time,
                            Year = startedDateForYearly.Year
                        });
                    }
                }
                else if (evt.Repeat == CalEventRepeat.EveryXdays)
                {
                    var startedDateForCurrent = evt.Started;

                    while (startedDateForCurrent <= sheetLastDay)
                    {
                        if (startedDateForCurrent >= sheetFirstDay)
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
                                Started = startedDateForCurrent.Date,
                                Status = evt.Status,
                                Time = evt.Time,
                                Year = (evt.Repeat == CalEventRepeat.Yearly) ? 0 : evt.Started.Year
                            });
                        }
                        // date increment.
                        startedDateForCurrent = startedDateForCurrent.AddDays(evt.EveryXDays.Value);
                    }
                }
                else
                {
                    eventsModel.Add(evt);
                }
            }
            return eventsModel;
        }
    }
}
