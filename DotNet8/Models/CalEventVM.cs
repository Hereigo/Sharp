namespace DotNet8.Models
{
    public class CalEventVM
    {
        public CalEventVM(CalEvent calEvent)
        {
            var time = calEvent.Started.ToString("HH:mm");

            Id = calEvent.Id;
            DayOfMonth = calEvent.Started.Day;
            Description = calEvent.Description;
            Time = time == "00:00" ? string.Empty : (time + " - ");
        }
        public readonly int? Id;
        public readonly int DayOfMonth;
        public readonly string Description;
        public readonly string Time;
    }
}
