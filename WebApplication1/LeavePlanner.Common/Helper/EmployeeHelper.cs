namespace Common.Helper
{
    public static class EmployeeHelper
    {
        private const int BaseLeaveDays = 30;

        public static int CalculateLeaveDays(DateTime employeeDate)
        {
            var currentDate = DateTime.UtcNow;
            var yearsWorked = currentDate.Year - employeeDate.Year;
            int leaveDays;
            
            if (currentDate.Month >= employeeDate.Month && currentDate.Day >= employeeDate.Day)
            {
                leaveDays = yearsWorked;
            }
            else
            {
                leaveDays = yearsWorked - 1;
            }

            return BaseLeaveDays + leaveDays;
        }
    }
}
