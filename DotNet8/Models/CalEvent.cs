using System.ComponentModel;

namespace DotNet8.Models
{
    public enum CalEventStatus
    {
        [Description("Active")]
        Active,
        [Description("Disabled")]
        Disabled,
        [Description("Deleted")]
        Deleted
    }

    // Description = Enumerations.GetEnumDescription((MyEnum)value);

    public enum CalEventRepeat
    {
        [Description("Once")]
        Once,
        [Description("Yearly")]
        Yearly,
        [Description("Monthly")]
        Monthly,
        [Description("Every X days")]
        EveryXdays,
    }

    public class CalEvent
    {
        public int Id { get; set; }

        public CalEventCategory Category { get; set; }
        public CalEventRepeat Repeat { get; set; }
        public CalEventStatus Status { get; set; }

        public DateTime Modified { get; set; }
        public DateTime Started { get; set; }

        public TimeSpan Time { get; set; }
        
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int? EveryXDays { get; set; }
        
        public string Description { get; set; }

        public CalEvent() { }

        public CalEvent(DateTime date)
        {
            Day = date.Day;
            EveryXDays = null;
            Modified = date;
            Month = date.Month;
            Repeat = CalEventRepeat.Once;
            Started = date;
            Status = CalEventStatus.Active;
            Year = date.Year;
        }

        public CalEvent(string description, CalEventCategory category)
        {
            var now = DateTime.Now;
            Category = category;
            Day = now.Day;
            Description = description;
            EveryXDays = null;
            Modified = now;
            Month = now.Month;
            Repeat = CalEventRepeat.Once;
            Started = now;
            Status = CalEventStatus.Active;
            Year = now.Year;
        }
    }

    public class CalEventCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public CalEventCategory()
        {
            Name = "default";
        }

        public CalEventCategory(string categoryName)
        {
            Name = categoryName;
        }
    }
}
