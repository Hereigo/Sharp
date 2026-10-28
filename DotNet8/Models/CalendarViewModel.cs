namespace Calendarium.Models;

public record CalendarDay(DateTime Date, IReadOnlyList<CalEvent> Events);

public class CalendarViewModel
{
    /// <summary>The real current date in the calendar's time zone.</summary>
    public required DateTime Today { get; init; }

    /// <summary>A date inside the month being displayed.</summary>
    public required DateTime DisplayedMonth { get; init; }

    /// <summary>All days of the sheet (6 weeks, starting on Monday).</summary>
    public required IReadOnlyList<CalendarDay> Days { get; init; }

    public required int SheetEventsCount { get; init; }

    public required int TotalEventsCount { get; init; }

    public required bool IsDevelopment { get; init; }
}
