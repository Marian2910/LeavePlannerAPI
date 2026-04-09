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
        private const string ActionLogMessage = "{MethodName} was called from {ControllerName}";
        private const string CustomerAddedMessage = "The customer was added!";
        private const string InvalidModelStateLogMessage = "Invalid model state. {ModelState}";
        private const string CustomerUpdatedLogMessage = "The customer with id {CustomerId} was updated successfully!";
        private const string CustomerInactiveMessage = "The customer was made inactive!";
        private const string CustomersInactiveMessage = "The customers were made inactive";
        private const string DocumentsAddedMessage = "The documents were added to the customer!";

        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<Customer>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResultDto<Customer>>> GetAllCustomers(
            int pageNumber, 
            int pageSize, 
            string? sortDirection = null, 
            string? sortCriteria = null, 
            bool? status = null, 
            string? search = null)
        {
            logger.LogInformation(ActionLogMessage, nameof(GetAllCustomers), nameof(CustomerController));
            var customers = await customerService.GetCustomersAsync(pageNumber, pageSize, sortDirection, sortCriteria, status, search);

            return Ok(customers);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            logger.LogInformation("{GetCustomerByIdName} was called from {CustomerControllerName}, Customer Id: {Id}", nameof(GetCustomerById), nameof(CustomerController), id);
            var customer = await customerService.GetCustomerByIdAsync(id);
            return Ok(customer);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerDto customerDto)
        {
            logger.LogInformation(ActionLogMessage, nameof(AddCustomer), nameof(CustomerController));
            if (ModelState.IsValid)
            {
                var customer = mapper.Map<Customer>(customerDto);
                await customerService.AddCustomer(customer);
                logger.LogInformation(CustomerAddedMessage);
                return Ok(new { message = CustomerAddedMessage });
            }
            logger.LogInformation(InvalidModelStateLogMessage, ModelState);
            return BadRequest(ModelState);
        }

        [HttpPost("add-documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddDocumentToCustomer([FromForm] int id, IEnumerable<IFormFile> files)
        {
            logger.LogInformation(ActionLogMessage, nameof(AddDocumentToCustomer), nameof(CustomerController));
            if (ModelState.IsValid)
            {
                await customerService.AddDocumentsToCustomer(id, files);
                return Ok(DocumentsAddedMessage);
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerDto customerDto)
        {
            logger.LogInformation(ActionLogMessage, nameof(UpdateCustomer), nameof(CustomerController));
            if (ModelState.IsValid)
            {
                var customer = mapper.Map<Customer>(customerDto);
                await customerService.UpdateCustomer(customer);

                logger.LogInformation(CustomerUpdatedLogMessage, customer.Id);
                return Ok($"The customer with id {customerDto.Id} was updated successfully!");
            }
            logger.LogInformation(InvalidModelStateLogMessage, ModelState);
            return BadRequest(ModelState);
        }

        [HttpDelete("{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCustomer(int customerId)
        {
            logger.LogInformation(ActionLogMessage, nameof(DeleteCustomer), nameof(CustomerController));
            await customerService.DeleteCustomer(customerId);
            logger.LogInformation(CustomerInactiveMessage);
            return Ok(CustomerInactiveMessage);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResultDto<Customer>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResultDto<Customer>>> SearchCustomersByName(string name, int pageNumber, int pageSize)
        {
            logger.LogInformation("{SearchCustomersByNameName} was called from {CustomerControllerName}. Trying to search customers by name: {Name}", nameof(SearchCustomersByName), nameof(CustomerController), name);
            var pagedCustomers = await customerService.SearchCustomersByNameAsync(name, pageNumber, pageSize);
            return Ok(pagedCustomers);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMultipleCustomers([FromBody] int[] ids)
        {
            logger.LogInformation(ActionLogMessage, nameof(DeleteMultipleCustomers), nameof(CustomerController));
            await customerService.DeleteMultipleCustomers(ids);
            return Ok(CustomersInactiveMessage);
        }
    }
}
