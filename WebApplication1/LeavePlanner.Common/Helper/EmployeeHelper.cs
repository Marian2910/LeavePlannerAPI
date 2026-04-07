namespace Common.Helper
{
    public static class EmployeeHelper
    {
        public static int CalculateLeaveDays(DateTime employeeDate)
        {
            var yearsWorked = DateTime.Now.Year - employeeDate.Year;
            int leaveDays;
            
            if(DateTime.Now.Month >= employeeDate.Month && DateTime.Now.Day >= employeeDate.Day)
            {
                leaveDays = yearsWorked;
            }
            else
            {
                leaveDays = yearsWorked - 1;
            }

            var baseLeaveDays = 30;

            return baseLeaveDays + leaveDays;
        }
    }
}
