using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeavePlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController(EmployeeService employeeService, ILogger<EmployeeController> logger) : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
    public async Task<ActionResult<Employee>> GetEmployeeById(int id)
    {
        logger.LogInformation("Fetching employee details for Employee ID: {Id}", id);

        var employee = await employeeService.GetEmployeeByIdAsync(id);

        logger.LogInformation("Employee details successfully retrieved for Employee ID: {Id}", id);
        return Ok(employee);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
    {
        logger.LogInformation("Fetching all employees.");

        var employees = await employeeService.GetAllEmployeesAsync();

        // Check if employees received is null or empty
        if (!employees.Any())
        {
            logger.LogInformation("No employees found.");
            return NotFound(new { message = "No employees found." });
        }

        logger.LogInformation("Successfully retrieved all employees.");
        return Ok(employees);
    }
}
