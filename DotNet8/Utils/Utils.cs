using DotNet8.Models;

namespace DotNet8.Utils
{
    public static class Utils
    {
        public static int GetMaxDayOfTheMonth(DateTime now)
        {
            return new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1).Day;
        }

        public static List<CalEventVM> GetCalEventVm(List<CalEventVM>? calEvent, int day)
        {
            if (calEvent?.Count > 0)
            {
                return calEvent;
            }
            else
            {
                return new List<CalEventVM> {
                    // new CalEventVM(new CalEvent() { Started = new DateTime(2024, 07, day) })
                };
            }
        }
    }
}
