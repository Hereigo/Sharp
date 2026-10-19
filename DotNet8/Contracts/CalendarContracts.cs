using System.ComponentModel.DataAnnotations;
using Calendarium.Models;

namespace Calendarium.Contracts
{
    public sealed record CalEventDto(
        int Id,
        DateOnly Started,
        TimeSpan Time,
        string Description,
        CalEventRepeat Repeat,
        int? EveryXDays,
        CalEventStatus Status,
        int Day,
        int Month,
        int Year,
        DateTime? Created,
        DateTime Modified)
    {
        public static CalEventDto From(CalEvent e) => new(
            e.Id, DateOnly.FromDateTime(e.Started), e.Time, e.Description, e.Repeat, e.EveryXDays,
            e.Status, e.Day, e.Month, e.Year, e.Created, e.Modified);
    }

    public sealed record CalendarSheetDto(
        DateOnly Today,
        int Year,
        int Month,
        DateOnly FirstDay,
        DateOnly LastDay,
        IReadOnlyList<CalEventDto> Events,
        int TotalCount,
        bool ImportEnabled);

    public sealed class CalEventInput : IValidatableObject
    {
        [Required, StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public DateOnly Started { get; set; }

        public TimeSpan Time { get; set; }

        public CalEventRepeat Repeat { get; set; }

        public int? EveryXDays { get; set; }

        public CalEventStatus Status { get; set; } = CalEventStatus.Active;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Repeat == CalEventRepeat.EveryXdays && (EveryXDays == null || EveryXDays < 1 || EveryXDays > 777))
            {
                yield return new ValidationResult("Every X days - must be between 1 and 777 for the repeating.", [nameof(EveryXDays)]);
            }
            if (Time < TimeSpan.Zero || Time >= TimeSpan.FromDays(1))
            {
                yield return new ValidationResult("Time must be within a day.", [nameof(Time)]);
            }
        }
    }

    public sealed record HistoryItemDto(DateTime Created, ReqHeadFieldType Field, string Text);
}
