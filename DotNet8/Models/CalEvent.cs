using System.ComponentModel.DataAnnotations;

namespace Calendarium.Models;

public enum CalEventStatus
{
    Active,
    Disabled,
    Deleted
}

public enum CalEventRepeat
{
    Once,
    Yearly,
    Monthly,
    EveryXdays
}

public class CalEvent : IValidatableObject
{
    public const int MaxEveryXDays = 777;

    public int Id { get; set; }

    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int? EveryXDays { get; set; }

    public CalEventCategory? Category { get; set; }
    public CalEventRepeat Repeat { get; set; }
    public CalEventStatus Status { get; set; }

    [DataType(DataType.Date)]
    public DateTime? Created { get; set; }

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = @"{0:dd.MM.yy _ HH:mm}")]
    public DateTime Modified { get; set; }

    [DataType(DataType.Date)]
    public DateTime Started { get; set; }

    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
    public TimeSpan Time { get; set; }

    public string Description { get; set; } = string.Empty;

    public CalEvent()
    {
    }

    public CalEvent(DateTime date, string text = "")
    {
        Day = date.Day;
        Description = text;
        EveryXDays = null;
        Modified = date;
        Month = date.Month;
        Repeat = CalEventRepeat.Once;
        Started = date;
        Status = CalEventStatus.Active;
        Year = date.Year;
    }

    /// <summary>
    /// Copies the user-editable fields from <paramref name="source"/> and derives Day/Month/Year from its start date
    /// (Month is 0 for monthly events, Year is 0 for yearly events).
    /// </summary>
    public void CopyEditableFieldsFrom(CalEvent source)
    {
        Description = source.Description;
        EveryXDays = source.EveryXDays;
        Repeat = source.Repeat;
        Started = source.Started;
        Status = source.Status;
        Time = source.Time;
        Day = source.Started.Day;
        Month = source.Repeat == CalEventRepeat.Monthly ? 0 : source.Started.Month;
        Year = source.Repeat == CalEventRepeat.Yearly ? 0 : source.Started.Year;
        Modified = DateTime.Now;
    }

    /// <summary>
    /// Creates a display-only copy of a repeating event that occurs on <paramref name="date"/>.
    /// </summary>
    public CalEvent CreateOccurrence(DateTime date) => new()
    {
        Id = Id,
        Day = date.Day,
        Description = Description,
        EveryXDays = EveryXDays,
        Modified = Modified,
        Month = date.Month,
        Repeat = Repeat,
        Started = date,
        Status = Status,
        Time = Time,
        Year = date.Year
    };

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Repeat == CalEventRepeat.EveryXdays && (EveryXDays == null || EveryXDays < 1 || EveryXDays > MaxEveryXDays))
        {
            yield return new ValidationResult("Every X days - must be setup for the repeating.", new[] { nameof(Repeat) });
        }
    }
}

public class CalEventCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = "default";

    public CalEventCategory()
    {
    }

    public CalEventCategory(string categoryName)
    {
        Name = categoryName;
    }
}
