using AutoMapper;
using Common.DTOs;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace ProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly EmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(EmployeeService employeeService, IMapper mapper, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            _logger.LogInformation($"{nameof(GetEmployeeById)} was called from {nameof(EmployeeController)}");

            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            return Ok(employee);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<Employee>>> GetAllEmployees()
        {
            _logger.LogInformation($"{nameof(GetAllEmployees)} was called from {nameof(EmployeeController)}");

            var employees = await _employeeService.GetAllEmployeesAsync();

            return Ok(employees);
        }
    }
}
