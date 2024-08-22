using DotNet8.Models;

namespace DotNet8.Utils
{
    public static class Utils
    {
        public static int GetMaxDayOfTheMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1).Day;
        }

        public static List<CalEvent> GetCalEventVm(List<CalEvent>? calEvent, int day)
        {
            if (calEvent?.Count > 0)
            {
                return calEvent;
            }
            else
            {
                return new List<CalEvent> { };
            }
        }
    }
}
