using AutoMapper;
using Common.Helper;
using LeavePlanner.Domain.Models;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace LeavePlanner.Domain.Services
{
    public class EmployeeService(
        IEmployeeRepository employeeRepository,
        IMapper mapper,
        ILogger<EmployeeService> logger)
    {
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            logger.LogInformation("Fetching all employees.");

            var employeeEntities = await employeeRepository.GetAllEmployeeAsync();

            return mapper.Map<IEnumerable<Employee>>(employeeEntities);
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            logger.LogInformation("Fetching employee with Id {EmployeeId}.", id);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            var employeeEntity = await employeeRepository.GetByIdAsync(id);

            if (employeeEntity == null)
            {
                throw new KeyNotFoundException($"Employee with ID {id} was not found.");
            }

            return mapper.Map<Employee>(employeeEntity);
        }

        public async Task UpdateLeaveDaysForEmployeeAsync(int employeeId)
        {
            logger.LogInformation("Updating leave days for employee with Id {EmployeeId}.", employeeId);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(employeeId);

            var employee = await employeeRepository.GetByIdAsync(employeeId);

            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {employeeId} was not found.");
            }

            var expectedLeaveDays = EmployeeHelper.CalculateLeaveDays(
                employee.EmploymentDate);

            if (employee.AnnualLeaveDays >= expectedLeaveDays)
            {
                logger.LogInformation(
                    "No update needed for employee {EmployeeId}. Current leave days: {CurrentLeaveDays}, Expected: {ExpectedLeaveDays}.",
                    employeeId,
                    employee.AnnualLeaveDays,
                    expectedLeaveDays);

                return;
            }

            employee.AnnualLeaveDays = expectedLeaveDays;

            await employeeRepository.UpdateEmployeeAsync(employee);

            logger.LogInformation(
                "Updated leave days for employee {EmployeeId} to {NewLeaveDays}.",
                employeeId,
                expectedLeaveDays);
        }
    }
}
