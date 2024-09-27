using Infrastructure.Entities;

namespace Infrastructure.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task UpdateCustomerAsync(Customer customer);
        Task<IEnumerable<Customer>> SearchCustomersAsync(string name);
        Task<int> GetTotalCustomerCountAsync();
        Task<IEnumerable<Customer>> FilterCustomersByStatusAsync(bool status, int pageNumber, int pageSize);
        Task DeleteMultipleCustomers(int[] id);
    }
}