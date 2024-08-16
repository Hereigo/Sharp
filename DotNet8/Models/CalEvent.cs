namespace DotNet8.Models
{
    public enum CalEventStatus
    {
        Active,
        Disabled,
        Deleted
    }

    public enum CalEventRepeat
    {
        ONCE,
        YEARLY,
        MONTHLY,
        EVERYXDAYS,
    }

    public class CalEvent
    {
        public int Id { get; set; }
        public CalEventCategory Category { get; set; }
        public CalEventRepeat Repeat { get; set; }
        public CalEventStatus Status { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Started { get; set; }
        public int Day { get; set; }
        public int? EveryXDays { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }

        public CalEvent() { }

        public CalEvent(string description, CalEventCategory category)
        {
            var now = DateTime.Now;
            Category = category;
            Day = now.Day;
            Description = description;
            EveryXDays = null;
            Modified = now;
            Month = now.Month;
            Repeat = CalEventRepeat.ONCE;
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
