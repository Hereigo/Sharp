namespace DotNet8.Models
{
    public class CalEvent
    {
        public int Id { get; set; }
        public int PeriodSize { get; set; }
        public CalEventCategory Category { get; set; }
        public CalEventPeriod Period { get; set; }
        public CalEventStatus Status { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Started { get; set; }
        public string Description { get; set; }
    }
}
