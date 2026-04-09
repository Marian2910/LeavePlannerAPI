using LeavePlanner.Infrastructure.Interfaces;
using LeavePlanner.Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using LeavePlanner.Infrastructure.Configuration;
using EmployeeEntity = LeavePlanner.Infrastructure.Entities.Employee;

namespace LeavePlanner.Infrastructure.Repositories
{
    public class EmployeeRepository(ApplicationDbContext dbContext, ILogger<EmployeeRepository> logger) : IEmployeeRepository
    {
        public async Task<EmployeeEntity> GetByIdAsync(int id)
        {
            logger.LogInformation("Fetching employee with Id {EmployeeId}.", id);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            var employee = await dbContext.Employees
                .Include(e => e.Job)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                logger.LogWarning("Employee with Id {EmployeeId} not found.", id);
            }

            await Validator.ValidEntity(employee, logger);

            return employee ?? throw new InvalidOperationException();
        }

        public async Task<IEnumerable<EmployeeEntity>> GetAllEmployeeAsync()
        {
            logger.LogInformation("Fetching all employees.");

            var employees = await dbContext.Employees
                .Include(e => e.Job)
                .Include(e => e.Department)
                .ToListAsync();

            await Validator.ValidEntities(employees, logger);

            return employees;
        }

        public async Task UpdateEmployeeAsync(EmployeeEntity? employee)
        {
            logger.LogInformation("Updating employee with Id {EmployeeId}.", employee?.Id);

            ArgumentNullException.ThrowIfNull(employee);

            await Validator.ValidEntity(employee, logger);

            dbContext.Employees.Update(employee);
            await dbContext.SaveChangesAsync();
        }
    }
}
