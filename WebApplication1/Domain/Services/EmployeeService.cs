using AutoMapper;
using Common.Helper;
using Domain.Models;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Domain.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper, ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            _logger.LogInformation("GetAllEmployeesAsync was called from EmployeeService.");

            var employees = await _employeeRepository.GetAllEmployeeAsync();

            return _mapper.Map<IEnumerable<Employee>>(employees);
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            _logger.LogInformation("GetEmployeeByIdAsync was called from EmployeeService.");

            var employeeEntity = await _employeeRepository.GetByIdAsync(id);

            return _mapper.Map<Employee>(employeeEntity);
        }

        public async Task UpdateLeaveDaysForEmployeeAsync(int employeeId)
        {
            _logger.LogInformation("UpdateLeaveDaysForEmployeeAsync was called from EmployeeService.");

            var employee = await _employeeRepository.GetByIdAsync(employeeId);

            if (employee != null)
            {
                var expectedLeaveDays = EmployeeHelper.CalculateLeaveDays(employee.EmploymentDate, employee.RemainingLeaveDays);

                if (employee.AnnualLeaveDays < expectedLeaveDays)
                {
                    employee.AnnualLeaveDays = expectedLeaveDays;
                    await _employeeRepository.UpdateEmployeeAsync(employee);
                }
            }

        }
    }
}
