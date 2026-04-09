using LeavePlanner.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LeavePlanner.Domain.Services.Schedulers
{
    public class AnniversaryScheduler : IHostedService, IDisposable
    {
        private const int DefaultLeaveDays = 30;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AnniversaryScheduler> _logger;
        private Timer _timer;
        private bool _disposed;

        public AnniversaryScheduler(IServiceProvider serviceProvider, ILogger<AnniversaryScheduler> logger, Timer timer)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(timer);

            _serviceProvider = serviceProvider;
            _logger = logger;
            _timer = timer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(PerformScheduledTask, null, TimeSpan.Zero, TimeSpan.FromDays(30));
            _logger.LogInformation("AnniversaryScheduler has started running.");
            return Task.CompletedTask;
        }

        private void PerformScheduledTask(object? state)
        {
            Task.Run(async () => await PerformScheduledTaskAsync())
                .ContinueWith(task =>
                {
                    // Log any exception that occurs during the background work
                    if (task.Exception != null)
                    {
                        _logger.LogError(task.Exception, "An error occurred while executing the scheduled task.");
                    }
                });
        }

        private async Task PerformScheduledTaskAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var employeeService = scope.ServiceProvider.GetRequiredService<EmployeeService>();

            try
            {
                var employees = await employeeService.GetAllEmployeesAsync();
                foreach (var employee in employees)
                {
                    if (!HasLeaveBeenUpdatedThisYear(employee))
                    {
                        _logger.LogInformation("Updating leave days for Employee ID: {EmployeeId}", employee.Id);
                        await employeeService.UpdateLeaveDaysForEmployeeAsync(employee.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions during execution
                _logger.LogError(ex, "An unexpected error occurred while updating employees' leave days.");
                throw new InvalidOperationException("An error occurred in the AnniversaryScheduler.", ex);
            }
        }

        private static bool HasLeaveBeenUpdatedThisYear(Employee employee)
        {
            Debug.Assert(employee != null, "The employee parameter cannot be null.");
            
            int extraLeaveDays = DateTime.Today.Year - employee.EmploymentDate.Year;
            int expectedLeaveDays = DefaultLeaveDays + Math.Max(0, extraLeaveDays);

            return employee.AnnualLeaveDays >= expectedLeaveDays;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("AnniversaryScheduler is stopping.");
            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _timer.Dispose();
                _logger.LogInformation("AnniversaryScheduler resources have been disposed.");
            }

            _disposed = true;
        }
    }
}
