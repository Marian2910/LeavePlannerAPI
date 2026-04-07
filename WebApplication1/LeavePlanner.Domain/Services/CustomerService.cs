using AutoMapper;
using Common.DTOs;
using LeavePlanner.Domain.Helper;
using LeavePlanner.Domain.Models;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DocumentEntity = LeavePlanner.Infrastructure.Entities.Document;
using CustomerEntity = LeavePlanner.Infrastructure.Entities.Customer;

namespace LeavePlanner.Domain.Services
{
    public class CustomerService(
        ICustomerRepository customerRepository,
        IMapper mapper,
        ILogger<CustomerService> logger)
    {
        public async Task AddCustomer(Customer customer)
        {
            logger.LogInformation("Executing {MethodName}", nameof(AddCustomer));
            var customerEntity = mapper.Map<CustomerEntity>(customer);

            await customerRepository.AddCustomerAsync(customerEntity);
        }

        public async Task UpdateCustomer(Customer customer)
        {
            logger.LogInformation("Executing {MethodName} for Customer ID: {CustomerId}", nameof(UpdateCustomer), customer.Id);

            await ValidationHelper.ValidCustomerExists(customer.Id, customerRepository, logger);
            var updatedCustomerEntity = mapper.Map<CustomerEntity>(customer);

            await customerRepository.UpdateCustomerAsync(updatedCustomerEntity);
        }

        public async Task AddDocumentsToCustomer(int id, IEnumerable<IFormFile> formFiles)
        {
            logger.LogInformation("Executing {MethodName} for Customer ID: {CustomerId}", nameof(AddDocumentsToCustomer), id);

            await ValidationHelper.ValidCustomerExists(id, customerRepository, logger);

            var customerEntity = await customerRepository.GetByIdAsync(id)
                                    ?? throw new InvalidOperationException($"Customer with ID {id} not found.");
            
            foreach (var file in formFiles)
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                if (memoryStream.Length == 0 || 
                    (file.ContentType != "application/pdf" && file.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document" && file.ContentType != "application/msword"))
                {
                    throw new ArgumentException("Only PDF or DOCX files are supported.");
                }

                var document = new DocumentEntity
                {
                    Name = file.FileName,
                    Type = file.ContentType,
                    CreatedAt = DateTime.Now,
                    File = memoryStream.ToArray(),
                    Customer = customerEntity,
                };

                customerEntity.Documents?.Add(document);
            }

            await customerRepository.UpdateCustomerAsync(customerEntity);
        }

        public async Task DeleteCustomer(int customerId)
        {
            logger.LogInformation("Executing {MethodName} for Customer ID: {CustomerId}", nameof(DeleteCustomer), customerId);

            await ValidationHelper.ValidCustomerExists(customerId, customerRepository, logger);

            await customerRepository.DeleteCustomerAsync(customerId);
        }

        public async Task<PagedResultDto<Customer>> GetCustomersAsync(int pageNumber, int pageSize, string? sortDirection = null, string? sortCriteria = null, bool? status = null, string? search = null)
        {
            logger.LogInformation("Executing {MethodName}", nameof(GetCustomersAsync));

            await ValidationHelper.ValidPagination(pageNumber, pageSize, logger);

            var customerEntities = search != null
                ? await customerRepository.SearchCustomersAsync(search)
                : await customerRepository.GetAllAsync();

            var customers = mapper.Map<IEnumerable<Customer>>(customerEntities);

            if (sortCriteria != null && sortDirection != null)
            {
                customers = SortCustomers(customers, sortCriteria, sortDirection);
            }

            if (status.HasValue)
            {
                customers = customers.Where(c => c.Status == status.Value);
            }

            var totalCount = await customerRepository.GetTotalCustomerCountAsync();
            var pagedCustomers = GetPagedResults(customers, pageNumber, pageSize);

            return new PagedResultDto<Customer>
            {
                Items = pagedCustomers.ToList(),
                TotalCount = totalCount
            };
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            logger.LogInformation("Executing {MethodName} for Customer ID: {CustomerId}", nameof(GetCustomerByIdAsync), id);

            var customerEntity = await customerRepository.GetByIdAsync(id)
                                ?? throw new InvalidOperationException($"Customer with ID {id} not found.");

            return mapper.Map<Customer>(customerEntity);
        }

        public async Task<PagedResultDto<Customer>> SearchCustomersByNameAsync(string name, int pageNumber, int pageSize)
        {
            logger.LogInformation("Executing {MethodName} with Search Term: {SearchTerm}", nameof(SearchCustomersByNameAsync), name);

            await ValidationHelper.ValidPagination(pageNumber, pageSize, logger);

            var customerEntities = await customerRepository.SearchCustomersAsync(name);
            var customers = mapper.Map<IEnumerable<Customer>>(customerEntities);

            var pagedCustomers = GetPagedResults(customers, pageNumber, pageSize);

            return new PagedResultDto<Customer>
            {
                Items = pagedCustomers.ToList(),
                TotalCount = customerEntities.Count()
            };
        }

        public async Task<IEnumerable<Customer>> FilterCustomersByStatusAsync(bool status, int pageNumber, int pageSize)
        {
            logger.LogInformation("Executing {MethodName} with Status: {Status}", nameof(FilterCustomersByStatusAsync), status);

            await ValidationHelper.ValidPagination(pageNumber, pageSize, logger);

            var filteredCustomers = await customerRepository.FilterCustomersByStatusAsync(status, pageNumber, pageSize);

            return mapper.Map<IEnumerable<Customer>>(filteredCustomers);
        }

        public async Task DeleteMultipleCustomers(int[] ids)
        {
            logger.LogInformation("Executing {MethodName} for Customer IDs: {CustomerIds}", nameof(DeleteMultipleCustomers), ids);

            await customerRepository.DeleteMultipleCustomers(ids);
        }

        private static IEnumerable<Customer> GetPagedResults(IEnumerable<Customer> customers, int pageNumber, int pageSize)
        {
            return customers.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IEnumerable<Customer> SortCustomers(IEnumerable<Customer> customers, string sortCriteria, string sortDirection)
        {
            return sortCriteria.ToLower() switch
            {
                "name" => sortDirection.ToLower() == "asc"
                        ? customers.OrderBy(c => c.Name)
                        : customers.OrderByDescending(c => c.Name),
                "date" => sortDirection.ToLower() == "asc"
                        ? customers.OrderBy(c => c.Date)
                        : customers.OrderByDescending(c => c.Date),
                _ => throw new ArgumentException($"Invalid sorting criteria: {sortCriteria}.")
            };
        }
    }
}
