using Calendarium.Data;
using Calendarium.Models;
using Microsoft.EntityFrameworkCore;

namespace Calendarium.Services
{
    public sealed record CalendarSheet(
        DateTime Today,
        DateTime ViewMonth,
        DateTime FirstDay,
        DateTime LastDay,
        List<CalEvent> Occurrences,
        int TotalCount);

    public class CalendarService(ApplicationDbContext context, TimeProvider timeProvider)
    {
        public const int SheetDays = 7 * 6;

        // "FLE Standard Time" (Kyiv) - .NET maps the Windows id to IANA on Linux hosts.
        private static readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

        public DateTime Today() => TimeZoneInfo.ConvertTimeFromUtc(timeProvider.GetUtcNow().UtcDateTime, _timeZone);

        public async Task<CalendarSheet> BuildSheetAsync(int year, int month, CancellationToken ct = default)
        {
            var (firstDay, lastDay) = GetSheetRange(year, month);
            var dayAfterSheet = lastDay.AddDays(1);

            // All already started events, with non-repeating ones limited to the sheet:
            var events = await context.CalEvents
                .Where(e => e.Started < dayAfterSheet && (e.Repeat != CalEventRepeat.Once || e.Started >= firstDay))
                .ToListAsync(ct);

            var totalCount = await context.CalEvents.CountAsync(ct);

            var occurrences = ExpandOccurrences(events, firstDay, lastDay)
                .OrderBy(e => e.Time == default)
                .ThenBy(e => e.Time)
                .ToList();

            return new CalendarSheet(Today(), new DateTime(year, month, 1), firstDay, lastDay, occurrences, totalCount);
        }

        /// <summary>6x7 sheet, weeks start on Monday.</summary>
        public static (DateTime FirstDay, DateTime LastDay) GetSheetRange(int year, int month)
        {
            var monthStart = new DateTime(year, month, 1);
            int daysBefore = ((int)monthStart.DayOfWeek + 6) % 7; // Mon = 0 .. Sun = 6
            var firstDay = monthStart.AddDays(-daysBefore);
            return (firstDay, firstDay.AddDays(SheetDays - 1));
        }

        /// <summary>Expands repeating events into concrete occurrences within [firstDay, lastDay].</summary>
        public static List<CalEvent> ExpandOccurrences(IEnumerable<CalEvent> events, DateTime firstDay, DateTime lastDay)
        {
            var result = new List<CalEvent>();

            foreach (var evt in events)
            {
                var started = evt.Started.Date;

                switch (evt.Repeat)
                {
                    case CalEventRepeat.Monthly:
                        for (var m = new DateTime(firstDay.Year, firstDay.Month, 1); m <= lastDay; m = m.AddMonths(1))
                        {
                            AddIfInRange(evt, ClampedDate(m.Year, m.Month, started.Day));
                        }
                        break;

                    case CalEventRepeat.Yearly:
                        for (int y = firstDay.Year; y <= lastDay.Year; y++)
                        {
                            AddIfInRange(evt, ClampedDate(y, started.Month, started.Day));
                        }
                        break;

                    case CalEventRepeat.EveryXdays:
                        int step = evt.EveryXDays ?? 0;
                        if (step < 1) break;

                        var date = started;
                        if (date < firstDay)
                        {
                            int skipSteps = ((firstDay - date).Days + step - 1) / step;
                            date = date.AddDays((long)skipSteps * step);
                        }
                        for (; date <= lastDay; date = date.AddDays(step))
                        {
                            AddIfInRange(evt, date);
                        }
                        break;

                    default:
                        AddIfInRange(evt, started);
                        break;
                }
            }
            return result;

            void AddIfInRange(CalEvent evt, DateTime date)
            {
                if (date >= firstDay && date <= lastDay && date >= evt.Started.Date)
                {
                    result.Add(ToOccurrence(evt, date));
                }
            }
        }

        /// <summary>Sets the schedule fields the way the calendar expects them (Month = 0 for monthly, Year = 0 for yearly).</summary>
        public static void ApplySchedule(CalEvent target, DateTime started, CalEventRepeat repeat, int? everyXDays)
        {
            target.Started = started;
            target.Repeat = repeat;
            target.EveryXDays = everyXDays;
            target.Day = started.Day;
            target.Month = repeat == CalEventRepeat.Monthly ? 0 : started.Month;
            target.Year = repeat == CalEventRepeat.Yearly ? 0 : started.Year;
        }

        // Clamps e.g. the 31st or Feb 29 to the last day of a shorter month.
        private static DateTime ClampedDate(int year, int month, int day) =>
            new(year, month, Math.Min(day, DateTime.DaysInMonth(year, month)));

        private static CalEvent ToOccurrence(CalEvent evt, DateTime date) => new()
        {
            Id = evt.Id,
            Day = date.Day,
            Description = evt.Description,
            EveryXDays = evt.EveryXDays,
            Modified = evt.Modified,
            Month = date.Month,
            Repeat = evt.Repeat,
            Started = date,
            Status = evt.Status,
            Time = evt.Time,
            Year = date.Year,
        };
    }
}
