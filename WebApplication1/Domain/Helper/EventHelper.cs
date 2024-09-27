using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Helper
{
    public class EventHelper
    {
        public static int getWorkDaysBetweenDates(DateTime startDate, DateTime? endDate)
        {
            if (startDate > endDate)
            {
                throw new Exception("Start date cannot be after the end date.");
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
