using AutoMapper;
using Common.DTOs;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace ProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(CustomerService customerService, IMapper mapper, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<Customer>>> GetAllCustomers(int pageNumber, int pageSize, string? sortDirection = null, string? sortCriteria = null, bool? status = null, string? search = null)
        {
            _logger.LogInformation($"{nameof(GetAllCustomers)} was called from {nameof(CustomerController)}");
            var customers = await _customerService.GetCustomersAsync(pageNumber, pageSize, sortDirection, sortCriteria, status, search);

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            _logger.LogInformation($"{nameof(GetCustomerById)} was called from {nameof(CustomerController)}, Customer Id: {id}");
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                _logger.LogInformation($"Customer with id {id} not found");
                return NotFound($"Customer with Id {id} not found.");
            }
            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerDto customerDto)
        {
            _logger.LogInformation($"{nameof(AddCustomer)} was called from {nameof(CustomerController)}");
            if (ModelState.IsValid)
            {
                var customer = _mapper.Map<Customer>(customerDto);
                await _customerService.AddCustomer(customer);
                _logger.LogInformation($"The customer was added!");
                return Ok(new { message = "The customer was added!" });
            }
            _logger.LogInformation($"{ModelState} could not be found.");
            return BadRequest(ModelState);
        }

        [HttpPost("add-documents")]
        public async Task<IActionResult> AddDocumentToCustomer([FromForm] int id, IEnumerable<IFormFile> files)
        {
            _logger.LogInformation($"{nameof(AddDocumentToCustomer)} was called from {nameof(CustomerController)}");
            if (ModelState.IsValid)
            {
                await _customerService.AddDocumentsToCustomer(id, files);
                return Ok("The documents were added to the customer!");
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCustomer(UpdateCustomerDto customerDto)
        {
            _logger.LogInformation($"{nameof(UpdateCustomer)} was called from {nameof(CustomerController)}");
            if (ModelState.IsValid)
            {
                var customer = _mapper.Map<Domain.Models.Customer>(customerDto);
                await _customerService.UpdateCustomer(customer);

                _logger.LogInformation($"The customer with id {customer.Id} was updated successfully!");
                return Ok($"The customer with id {customerDto.Id} was updated successfully!");
            }
            _logger.LogInformation($"{ModelState} could not be found.");
            return BadRequest(ModelState);
        }

        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteCustomer(int customerId)
        {
            _logger.LogInformation($"{nameof(DeleteCustomer)} was called from {nameof(CustomerController)}");
            await _customerService.DeleteCustomer(customerId);
            _logger.LogInformation("The customer was made inactive!");
            return Ok("The customer was made inactive!");
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResultDto<Customer>>> SearchCustomersByName(string name, int pageNumber, int pageSize)
        {
            _logger.LogInformation($"{nameof(SearchCustomersByName)} was called from {nameof(CustomerController)}. Trying to search customers by name: {name}");
            var pagedCustomers = await _customerService.SearchCustomersByNameAsync(name, pageNumber, pageSize);
            if (pagedCustomers == null)
            {
                _logger.LogInformation($"There are no customers with the name {name}.");
                return NotFound($"There are no customers with the name {name}.");
            }
            return Ok(pagedCustomers);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMultipleCustomers([FromBody] int[] id)
        {
            _logger.LogInformation($"{nameof(DeleteMultipleCustomers)} was called from {nameof(CustomerController)}");
            await _customerService.DeleteMulipleCustomers(id);
            return Ok("The customers were made inactive");
        }
    }
}
