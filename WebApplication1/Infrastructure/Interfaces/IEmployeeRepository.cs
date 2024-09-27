using Infrastructure.Entities;

namespace Infrastructure.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetByIdAsync(int id);
        Task<IEnumerable<Employee>> GetAllEmployeeAsync();
        Task UpdateEmployeeAsync(Employee employee);
    }
}