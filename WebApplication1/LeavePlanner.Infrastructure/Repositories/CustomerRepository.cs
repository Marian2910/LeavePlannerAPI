using LeavePlanner.Infrastructure.Interfaces;
using LeavePlanner.Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Db = LeavePlanner.Infrastructure.Configuration.ApplicationDbContext;
using CustomerEntity = LeavePlanner.Infrastructure.Entities.Customer;

namespace LeavePlanner.Infrastructure.Repositories
{
    public class CustomerRepository(Db dbContext, ILogger<CustomerRepository> logger) : ICustomerRepository
    {
        public async Task AddCustomerAsync(CustomerEntity customer)
        {
            logger.LogInformation("Adding customer.");

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            await Validator.ValidEntity(customer, logger);

            await dbContext.Customers.AddAsync(customer);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            logger.LogInformation("Deleting (soft) customer with Id {CustomerId}.", id);

            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var customer = await dbContext.Customers.FindAsync(id);

            if (customer == null)
            {
                logger.LogWarning("Customer with Id {CustomerId} not found.", id);
                return;
            }

            await Validator.ValidEntity(customer, logger);

            customer.Status = false;

            await dbContext.SaveChangesAsync(); // FIX: awaited
        }

        public async Task<IEnumerable<CustomerEntity>> GetAllAsync()
        {
            logger.LogInformation("Fetching all customers.");

            var customers = await dbContext.Customers
                .AsNoTracking()
                .ToListAsync();

            await Validator.ValidEntities(customers, logger);

            return customers;
        }

        public async Task<CustomerEntity> GetByIdAsync(int id)
        {
            logger.LogInformation("Fetching customer with Id {CustomerId}.", id);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            var customer = await dbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                logger.LogWarning("Customer with Id {CustomerId} not found.", id);
            }

            await Validator.ValidEntity(customer, logger);

            return customer ?? throw new InvalidOperationException();
        }

        public async Task UpdateCustomerAsync(CustomerEntity customer)
        {
            logger.LogInformation("Updating customer with Id {CustomerId}.", customer.Id);

            ArgumentNullException.ThrowIfNull(customer);

            await Validator.ValidEntity(customer, logger);

            dbContext.Customers.Update(customer);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CustomerEntity>> SearchCustomersAsync(string name)
        {
            logger.LogInformation("Searching customers by name {Name}.", name);

            if (string.IsNullOrWhiteSpace(name))
                return Enumerable.Empty<CustomerEntity>();

            var customers = await dbContext.Customers
                .Where(c => c.Name.Contains(name))
                .AsNoTracking()
                .ToListAsync();

            await Validator.ValidEntities(customers, logger);

            return customers;
        }

        public async Task<int> GetTotalCustomerCountAsync()
        {
            logger.LogInformation("Getting total customer count.");

            return await dbContext.Customers.CountAsync();
        }

        public async Task<IEnumerable<CustomerEntity>> FilterCustomersByStatusAsync(bool status, int pageNumber, int pageSize)
        {
            logger.LogInformation(
                "Filtering customers by status {Status}, page {PageNumber}, size {PageSize}.",
                status, pageNumber, pageSize);

            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentOutOfRangeException("status");

            var customers = await dbContext.Customers
                .AsNoTracking()
                .Where(c => c.Status == status)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            await Validator.ValidEntities(customers, logger);

            return customers;
        }

        public async Task DeleteMultipleCustomers(int[]? id)
        {
            logger.LogInformation("Deleting multiple customers.");

            if (id == null || id.Length == 0)
                return;

            var customers = await dbContext.Customers
                .Where(c => id.Contains(c.Id))
                .ToListAsync();

            if (customers.Count == 0)
            {
                logger.LogWarning("No customers found for bulk delete.");
                return;
            }

            foreach (var customer in customers)
            {
                customer.Status = false;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
