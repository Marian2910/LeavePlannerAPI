
namespace LeavePlanner.Domain.Helper
{
    public static class EventHelper
    {
        public static int GetWorkDaysBetweenDates(DateTime startDate, DateTime? endDate)
        {
            if (startDate > endDate)
            {
                throw new InvalidOperationException("Start date cannot be after the end date.");
            }

            int totalDays = 0;
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    totalDays++;
                }
            }

            return totalDays;
        }
    }
}
