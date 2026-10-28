using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using Calendarium.Data;
using Calendarium.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace Calendarium.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<HomeController> _logger;

        private const int _historyLines = 40;
        private const int _sheetDays = 7 * 6;
        private const long _maxUploadBytes = 2 * 1024 * 1024;

        private static readonly TimeZoneInfo _calendarTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

        // To record another header (e.g. Accept-Language, Referer) add it here.
        private static readonly (ReqHeadFieldType Field, string Header)[] _trackedHeaders =
        [
            (ReqHeadFieldType.Accept, HeaderNames.Accept),
            (ReqHeadFieldType.UsrAgent, HeaderNames.UserAgent),
        ];

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, IHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        [AllowAnonymous]
        public IActionResult Privacy() => View();

        [AllowAnonymous]
        public IActionResult StartPage() => PhysicalFile(Path.Combine(_hostEnvironment.ContentRootPath, "Index.html"), "text/html");

        [AllowAnonymous]
        public async Task<IActionResult> Index(string pMonth = "")
        {
            await TrackRequestHeadersAsync();

            DateTime today = CalendarToday();
            DateTime displayedMonth = pMonth switch
            {
                "next" => today.AddMonths(1),
                "prev" => today.AddMonths(-1),
                _ => today
            };

            var monthStart = new DateTime(displayedMonth.Year, displayedMonth.Month, 1);
            int daysSinceMonday = ((int)monthStart.DayOfWeek + 6) % 7;
            DateTime sheetFirstDay = monthStart.AddDays(-daysSinceMonday);
            DateTime sheetLastDay = sheetFirstDay.AddDays(_sheetDays - 1);

            // All already started repeating events plus the one-time events on this sheet:
            var events = await _context.CalEvents
                .Where(e => e.Started <= sheetLastDay && (e.Repeat != CalEventRepeat.Once || e.Started >= sheetFirstDay))
                .ToListAsync();

            var occurrences = ExpandOccurrences(events, sheetFirstDay, sheetLastDay, displayedMonth);

            var days = Enumerable.Range(0, _sheetDays)
                .Select(i => sheetFirstDay.AddDays(i))
                .Select(date => new CalendarDay(date, occurrences
                    .Where(e => e.Started.Date == date)
                    .OrderBy(e => e.Time == default) // Events with a time first, ordered by time.
                    .ThenBy(e => e.Time)
                    .ToList()))
                .ToList();

            return View(new CalendarViewModel
            {
                Today = today,
                DisplayedMonth = displayedMonth,
                Days = days,
                SheetEventsCount = occurrences.Count,
                TotalEventsCount = await _context.CalEvents.CountAsync(),
                IsDevelopment = _hostEnvironment.IsDevelopment(),
            });
        }

        public async Task<IActionResult> FullList()
        {
            var events = await _context.CalEvents.OrderBy(x => x.Month).ThenBy(x => x.Day).ToListAsync();
            return View(events);
        }

        public IActionResult Create(DateTime? date)
        {
            var newEvent = new CalEvent()
            {
                EveryXDays = 0,
                Modified = DateTime.Now,
                Started = (date ?? CalendarToday()).Date,
                Status = CalEventStatus.Active,
                Time = TimeSpan.Zero,
                Repeat = CalEventRepeat.Once,
            };
            return View(newEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CalEvent evnt)
        {
            if (!ModelState.IsValid)
            {
                return View(evnt);
            }

            var calEvent = new CalEvent { Created = DateTime.Now };
            calEvent.CopyEditableFieldsFrom(evnt);
            _context.Add(calEvent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var calEvent = await _context.CalEvents.FindAsync(id);
            return calEvent == null ? NotFound() : View(calEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CalEvent evnt)
        {
            if (id != evnt.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(evnt);
            }

            var calEvent = await _context.CalEvents.FindAsync(id);
            if (calEvent == null)
            {
                return NotFound();
            }
            calEvent.CopyEditableFieldsFrom(evnt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calEvent = await _context.CalEvents.FindAsync(id);
            if (calEvent != null)
            {
                _context.CalEvents.Remove(calEvent);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> History() => View(await _context.RequestsHeaders
            .Where(rh => rh.Field == ReqHeadFieldType.UsrAgent || rh.Field == ReqHeadFieldType.Language)
            .OrderByDescending(rh => rh.Created).Take(_historyLines).ToListAsync());

        public async Task<JsonResult> GetJson() => new JsonResult(
            await _context.CalEvents.ToArrayAsync(),
            new JsonSerializerOptions { PropertyNamingPolicy = null });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(IFormFile? uploadingFile)
        {
            if (!_hostEnvironment.IsDevelopment() || uploadingFile == null || uploadingFile.Length >= _maxUploadBytes)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await using var stream = uploadingFile.OpenReadStream();
                var importedEvents = await JsonSerializer.DeserializeAsync<List<CalEvent?>>(stream) ?? [];

                foreach (var evt in importedEvents)
                {
                    if (evt == null || !Validator.TryValidateObject(evt, new ValidationContext(evt), null, validateAllProperties: true))
                    {
                        _logger.LogWarning("Skipped invalid event '{Description}' while importing {FileName}.", evt?.Description, uploadingFile.FileName);
                        continue;
                    }

                    var calEvent = new CalEvent { Created = DateTime.Now };
                    calEvent.CopyEditableFieldsFrom(evt);
                    calEvent.Status = CalEventStatus.Active;
                    _context.CalEvents.Add(calEvent);
                }
                await _context.SaveChangesAsync();
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Cannot import events from {FileName}.", uploadingFile.FileName);
            }
            return RedirectToAction(nameof(Index));
        }

        private static DateTime CalendarToday() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _calendarTimeZone);

        private async Task TrackRequestHeadersAsync()
        {
            var dedupSince = DateTime.Now.AddHours(-1);

            foreach (var (field, header) in _trackedHeaders)
            {
                string? text = Request.Headers[header];
                if (string.IsNullOrWhiteSpace(text))
                {
                    continue;
                }

                bool recentlyRecorded = await _context.RequestsHeaders
                    .AnyAsync(old => old.Field == field && old.Text == text && old.Created > dedupSince);
                if (!recentlyRecorded)
                {
                    _context.RequestsHeaders.Add(new RequestHeaderField(field, text));
                }
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Expands repeating events into their concrete occurrences between <paramref name="sheetFirstDay"/> and <paramref name="sheetLastDay"/>.
        /// </summary>
        private static List<CalEvent> ExpandOccurrences(List<CalEvent> events, DateTime sheetFirstDay, DateTime sheetLastDay, DateTime displayedMonth)
        {
            var occurrences = new List<CalEvent>();

            foreach (var evt in events)
            {
                if (evt.Repeat == CalEventRepeat.Once)
                {
                    occurrences.Add(evt);
                    continue;
                }

                foreach (var date in OccurrenceDates(evt, sheetFirstDay, sheetLastDay, displayedMonth))
                {
                    if (date >= evt.Started.Date && date >= sheetFirstDay && date <= sheetLastDay)
                    {
                        occurrences.Add(evt.CreateOccurrence(date));
                    }
                }
            }
            return occurrences;
        }

        // Candidate dates may fall outside the sheet; the caller filters them.
        // AddMonths/AddYears clamp to the month end, so the 31st and February 29 are handled.
        private static IEnumerable<DateTime> OccurrenceDates(CalEvent evt, DateTime sheetFirstDay, DateTime sheetLastDay, DateTime displayedMonth)
        {
            DateTime started = evt.Started.Date;

            switch (evt.Repeat)
            {
                case CalEventRepeat.Monthly:
                    int monthsSinceStart = (displayedMonth.Year - started.Year) * 12 + displayedMonth.Month - started.Month;
                    for (int i = -1; i <= 1; i++) // The sheet covers parts of the previous and next months.
                    {
                        yield return started.AddMonths(monthsSinceStart + i);
                    }
                    break;

                case CalEventRepeat.Yearly:
                    for (int year = sheetFirstDay.Year; year <= sheetLastDay.Year; year++) // The sheet may span New Year.
                    {
                        yield return started.AddYears(year - started.Year);
                    }
                    break;

                case CalEventRepeat.EveryXdays:
                    int step = evt.EveryXDays.GetValueOrDefault();
                    if (step < 1)
                    {
                        yield break;
                    }
                    DateTime date = started;
                    if (date < sheetFirstDay) // Jump to the first occurrence on the sheet.
                    {
                        int stepsToSheet = (int)Math.Ceiling((sheetFirstDay - date).TotalDays / step);
                        date = date.AddDays(stepsToSheet * step);
                    }
                    for (; date <= sheetLastDay; date = date.AddDays(step))
                    {
                        yield return date;
                    }
                    break;
            }
        }
    }
}
