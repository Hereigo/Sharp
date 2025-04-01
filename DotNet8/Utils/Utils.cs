namespace Calendarium.Utils
{
    public static class Utils
    {
        public static int GetMaxDayOfTheMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1).Day;
        }
    }
}
