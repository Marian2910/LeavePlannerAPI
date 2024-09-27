using Infrastructure.Configuration;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<EmployeeRepository> _logger;

        public EmployeeRepository(ApplicationDBContext dbContext, ILogger<EmployeeRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            _logger.LogInformation($"{nameof(GetByIdAsync)} was called from {nameof(EmployeeRepository)}");

            var employeeToBeValidated = await _dbContext.Employees
                .Include(e => e.Job)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);

            await Validator.ValidEntity(employeeToBeValidated, _logger);

            return employeeToBeValidated;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeeAsync()
        {
            _logger.LogInformation($"{nameof(GetAllEmployeeAsync)} was called from {nameof(EmployeeRepository)}");

            var employees = await _dbContext.Employees.AsNoTracking()
                .Include(e => e.Job)
                .Include(e => e.Department)
                .ToListAsync();

            await Validator.ValidEntities(employees, _logger);

            return employees;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _dbContext.Employees.Update(employee);
            await _dbContext.SaveChangesAsync();
        }
    }
}
