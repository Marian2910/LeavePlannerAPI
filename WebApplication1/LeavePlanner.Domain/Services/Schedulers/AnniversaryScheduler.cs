using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Domain.Services.Schedulers
{
    public class AnniversaryScheduler : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public AnniversaryScheduler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(30));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Task.Run(async () => await DoWorkAsync());
        }

        private async Task DoWorkAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var employeeService = scope.ServiceProvider.GetRequiredService<EmployeeService>();

                var employees = await employeeService.GetAllEmployeesAsync();
                foreach (var employee in employees)
                {
                    if (!HasLeaveBeenUpdatedThisYear(employee))
                    {
                        await employeeService.UpdateLeaveDaysForEmployeeAsync(employee.Id);
                    }
                }
            }
        }

        private bool HasLeaveBeenUpdatedThisYear(Employee employee)
        {
            const int DefaultLeaveDays = 30;

            int extraLeaveDays = DateTime.Today.Year - employee.EmploymentDate.Year;

            int expectedLeaveDays = DefaultLeaveDays + Math.Max(0, extraLeaveDays);

            return employee.AnnualLeaveDays >= expectedLeaveDays;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
