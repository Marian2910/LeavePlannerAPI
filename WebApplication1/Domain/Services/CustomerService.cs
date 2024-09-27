using AutoMapper;
using Common.DTOs;
using Domain.Helper;
using Domain.Models;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Domain.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper, ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task AddCustomer(Customer customerDto)
        {
            _logger.LogInformation("AddCustomer was called from CustomerService");

            var customer = _mapper.Map<Infrastructure.Entities.Customer>(customerDto);

            await _customerRepository.AddCustomerAsync(customer);
        }


        public async Task UpdateCustomer(Customer customerDto)
        {
            _logger.LogInformation("UpdateCustomer was called from CustomerService");

            await ValidationHelper.ValidCustomerExists(customerDto.Id, _customerRepository, _logger);

            var updatedCustomer = _mapper.Map<Infrastructure.Entities.Customer>(customerDto);

            await _customerRepository.UpdateCustomerAsync(updatedCustomer);

        }

        public async Task AddDocumentsToCustomer(int id, IEnumerable<IFormFile> formFiles)
        {
            _logger.LogInformation("AddDocumentsToCustomer was called from CustomerService");

            await ValidationHelper.ValidCustomerExists(id, _customerRepository, _logger);

            var receivedCustomer = GetCustomerByIdAsync(id).Result;
            var customer = _mapper.Map<Infrastructure.Entities.Customer>(receivedCustomer);

            foreach (var file in formFiles)
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                if (memoryStream.Length > 0 && (file.ContentType == "application/pdf" || file.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || file.ContentType == "application/msword"))
                {
                    var fileByte = memoryStream.ToArray();
                    var document = new Infrastructure.Entities.Document
                    {
                        Name = file.FileName,
                        Type = file.ContentType,
                        Date = DateTime.Now,
                        File = fileByte,
                        Customer = customer,
                    };
                    customer.Documents.Add(document);
                }
                else
                {
                    throw new Exception("Only PDF or DOCX files!");
                }
            }

            await _customerRepository.UpdateCustomerAsync(customer);

        }

        public async Task DeleteCustomer(int customerId)
        {
            _logger.LogInformation("DeleteCustomer was called from CustomerService");

            await ValidationHelper.ValidCustomerExists(customerId, _customerRepository, _logger);

            await _customerRepository.DeleteCustomerAsync(customerId);
        }

        public async Task<PagedResultDto<Customer>> GetCustomersAsync(int pageNumber, int pageSize, string? sortDirection = null, string? sortCriteria = null, bool? status = null, string? search = null)
        {
            _logger.LogInformation("GetCustomerAsync was called from CustomerService");

            await ValidationHelper.ValidPagination(pageNumber, pageSize, _logger);

            var customerEntities = await _customerRepository.GetAllAsync();

            var customers = _mapper.Map<IEnumerable<Customer>>(customerEntities);

            var totalCount = await _customerRepository.GetTotalCustomerCountAsync();

            if (search != null) {
                customerEntities = await _customerRepository.SearchCustomersAsync(search);
                customers = _mapper.Map<IEnumerable<Customer>>(customerEntities);
            }

            if (sortDirection != null && sortCriteria != null)
            {
                customers = sortCriteria.ToLower() switch
                {
                    "name" => sortDirection.ToLower() == "asc" ? customers.OrderBy(c => c.Name).ToList() : customers.OrderByDescending(c => c.Name).ToList(),
                    "date" => sortDirection.ToLower() == "asc" ? customers.OrderBy(c => c.Date).ToList() : customers.OrderByDescending(c => c.Date).ToList(),
                    _ => throw new Exception("Invalid sorting criteria.")
                };
            }

            if (status != null) {
                customers = customers.Where(x => x.Status == status);
            }


            var pagedCustomers = GetAllPaged(customers, pageNumber, pageSize);

            var pagedResultDto = new PagedResultDto<Customer>
            {
                Items = _mapper.Map<IEnumerable<Customer>>(pagedCustomers),
                TotalCount = totalCount
            };

            return pagedResultDto;
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            _logger.LogInformation("GetCustomerByIdAsync was called from CustomerService");

            var customerEntity = await _customerRepository.GetByIdAsync(id);

            await ValidationHelper.ValidCustomerExists(id, _customerRepository, _logger);

            var customer = _mapper.Map<Domain.Models.Customer>(customerEntity);

            return _mapper.Map<Domain.Models.Customer>(customerEntity);
        }

        public async Task<PagedResultDto<Customer>> SearchCustomersByNameAsync(string name, int pageNumber, int pageSize)
        {
            _logger.LogInformation("SearchCustomersByNameAsync was called from CustomerService");

            var customerEntities = await _customerRepository.SearchCustomersAsync(name);

            var customers = _mapper.Map<IEnumerable<Customer>>(customerEntities);

            var pagedCustomers = GetAllPaged(customers, pageNumber, pageSize);

            var pagedResultDto = new PagedResultDto<Customer>
            {
                Items = _mapper.Map<List<Customer>>(pagedCustomers),
                TotalCount = customerEntities.Count()
            };

            return pagedResultDto;
        }

        public async Task<IEnumerable<Customer>> FilterCustomersByStatusAsync(bool status, int pageNumber, int pageSize)
        {
            _logger.LogInformation("FilterCustomersByStatusAsync was called from CustomerService");

            await ValidationHelper.ValidPagination(pageNumber, pageSize, _logger);

            var pagedFilteredCustomers = await _customerRepository.FilterCustomersByStatusAsync(status, pageNumber, pageSize);

            return _mapper.Map<IEnumerable<Customer>>(pagedFilteredCustomers);
        }

        public IEnumerable<Customer> GetAllPaged(IEnumerable<Customer> customers, int pageNumber, int pageSize)
        {
            _logger.LogInformation("GetAllPaged was called from CustomerRepository");

            return customers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task DeleteMulipleCustomers(int[] id)
        {
            _logger.LogInformation("DeleteMultipleCustomers was called from CustomerService");

            await _customerRepository.DeleteMultipleCustomers(id);
        }
    }
}
