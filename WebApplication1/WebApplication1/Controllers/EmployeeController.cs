using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeavePlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController(EmployeeService employeeService, ILogger<EmployeeController> logger) : ControllerBase
{
    private readonly EmployeeService _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
    private readonly ILogger<EmployeeController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployeeById(int id)
    {
        _logger.LogInformation("Fetching employee details for Employee ID: {Id}", id);

        var employee = await _employeeService.GetEmployeeByIdAsync(id);

        _logger.LogInformation("Employee details successfully retrieved for Employee ID: {Id}", id);
        return Ok(employee);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
    {
        _logger.LogInformation("Fetching all employees.");

        var employees = await _employeeService.GetAllEmployeesAsync();

        // Check if employees received is null or empty
        if (!employees.Any())
        {
            _logger.LogInformation("No employees found.");
            return NotFound(new { message = "No employees found." });
        }

        _logger.LogInformation("Successfully retrieved all employees.");
        return Ok(employees);
    }
}