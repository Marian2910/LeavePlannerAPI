using AutoMapper;
using Common.DTOs;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeavePlanner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController(CustomerService customerService, ILogger<CustomerController> logger, IMapper mapper)
        : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<Customer>>> GetAllCustomers(
            int pageNumber, 
            int pageSize, 
            string? sortDirection = null, 
            string? sortCriteria = null, 
            bool? status = null, 
            string? search = null)
        {
            logger.LogInformation($"{nameof(GetAllCustomers)} was called from {nameof(CustomerController)}");
            var customers = await customerService.GetCustomersAsync(pageNumber, pageSize, sortDirection, sortCriteria, status, search);

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            logger.LogInformation("{GetCustomerByIdName} was called from {CustomerControllerName}, Customer Id: {Id}", nameof(GetCustomerById), nameof(CustomerController), id);
            var customer = await customerService.GetCustomerByIdAsync(id);
            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerDto customerDto)
        {
            logger.LogInformation($"{nameof(AddCustomer)} was called from {nameof(CustomerController)}");
            if (ModelState.IsValid)
            {
                // Proper usage of _mapper integrated here
                var customer = mapper.Map<Customer>(customerDto);
                await customerService.AddCustomer(customer);
                logger.LogInformation($"The customer was added!");
                return Ok(new { message = "The customer was added!" });
            }
            logger.LogInformation($"Invalid model state. {ModelState}");
            return BadRequest(ModelState);
        }

        [HttpPost("add-documents")]
        public async Task<IActionResult> AddDocumentToCustomer([FromForm] int id, IEnumerable<IFormFile> files)
        {
            logger.LogInformation($"{nameof(AddDocumentToCustomer)} was called from {nameof(CustomerController)}");
            if (ModelState.IsValid)
            {
                await customerService.AddDocumentsToCustomer(id, files);
                return Ok("The documents were added to the customer!");
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerDto customerDto)
        {
            logger.LogInformation($"{nameof(UpdateCustomer)} was called from {nameof(CustomerController)}");
            if (ModelState.IsValid)
            {
                // Proper usage of _mapper integrated here
                var customer = mapper.Map<Customer>(customerDto);
                await customerService.UpdateCustomer(customer);

                logger.LogInformation($"The customer with id {customer.Id} was updated successfully!");
                return Ok($"The customer with id {customerDto.Id} was updated successfully!");
            }
            logger.LogInformation($"Invalid model state. {ModelState}");
            return BadRequest(ModelState);
        }

        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteCustomer(int customerId)
        {
            logger.LogInformation($"{nameof(DeleteCustomer)} was called from {nameof(CustomerController)}");
            await customerService.DeleteCustomer(customerId);
            logger.LogInformation("The customer was made inactive!");
            return Ok("The customer was made inactive!");
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResultDto<Customer>>> SearchCustomersByName(string name, int pageNumber, int pageSize)
        {
            logger.LogInformation("{SearchCustomersByNameName} was called from {CustomerControllerName}. Trying to search customers by name: {Name}", nameof(SearchCustomersByName), nameof(CustomerController), name);
            var pagedCustomers = await customerService.SearchCustomersByNameAsync(name, pageNumber, pageSize);
            return Ok(pagedCustomers);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMultipleCustomers([FromBody] int[] ids)
        {
            logger.LogInformation($"{nameof(DeleteMultipleCustomers)} was called from {nameof(CustomerController)}");
            await customerService.DeleteMultipleCustomers(ids);
            return Ok("The customers were made inactive");
        }
    }
}