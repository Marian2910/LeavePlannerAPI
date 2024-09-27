using Infrastructure.Configuration;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<CustomerRepository> _logger;

        public CustomerRepository(ApplicationDBContext dbContext, ILogger<CustomerRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            _logger.LogInformation($"{nameof(AddCustomerAsync)} was called from {nameof(CustomerRepository)}");

            await Validator.ValidEntity(customer, _logger);

            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            _logger.LogInformation($"{nameof(DeleteCustomerAsync)} was called from {nameof(CustomerRepository)}");

            var customer = await _dbContext.Customers.FindAsync(id);
            await Validator.ValidEntity(customer, _logger);
            if (customer != null)
            {
                customer.Status = false;
                _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            _logger.LogInformation($"{nameof(GetAllAsync)} was called from {nameof(CustomerRepository)}");

            var customersToBeValidated = await _dbContext.Customers.AsNoTracking().ToListAsync();
            await Validator.ValidEntities(customersToBeValidated, _logger);

            return customersToBeValidated;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            _logger.LogInformation($"{nameof(GetByIdAsync)} was called from {nameof(CustomerRepository)}");

            var customerToBeValidated = await _dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            await Validator.ValidEntity(customerToBeValidated, _logger);

            return customerToBeValidated;
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _logger.LogInformation($"{nameof(UpdateCustomerAsync)} was called from {nameof(CustomerRepository)}");

            await Validator.ValidEntity(customer, _logger);

            _dbContext.Customers.Update(customer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string name)
        {
            _logger.LogInformation($"{nameof(SearchCustomersAsync)} was called from {nameof(CustomerRepository)}");

            var customers = await _dbContext.Customers.Where(c => c.Name.Contains(name)).ToListAsync();
            await Validator.ValidEntities(customers, _logger);

            return customers;
        }

        public async Task<int> GetTotalCustomerCountAsync()
        {
            _logger.LogInformation($"{nameof(GetTotalCustomerCountAsync)} was called from {nameof(CustomerRepository)}");

            return await _dbContext.Customers.CountAsync();
        }

        public async Task<IEnumerable<Customer>> FilterCustomersByStatusAsync(bool status, int pageNumber, int pageSize)
        {
            _logger.LogInformation("FilterCustomersByStatusAsync was called from CustomerRepository");

            var customers = await _dbContext.Customers.AsNoTracking().Where(c => c.Status == status).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            await Validator.ValidEntities(customers, _logger);

            return customers;
        }

        public async Task DeleteMultipleCustomers(int[] id)
        {
            _logger.LogInformation("DeleteMultipleCustomers was called from CustomerRepository");

            var customers = await _dbContext.Customers.Where(c => id.Contains(c.Id)).ToListAsync();
            if (customers != null)
            {
                foreach (var customer in customers)
                {
                    customer.Status = false;
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
