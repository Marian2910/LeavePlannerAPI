using LeavePlanner.Api.Controllers;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Api.Controllers;

public class EmployeeControllerTests
{
    private readonly Mock<EmployeeService> _serviceMock;
    private readonly Mock<ILogger<EmployeeController>> _loggerMock = new();

    private readonly EmployeeController _controller;

    public EmployeeControllerTests()
    {
        _serviceMock = new Mock<EmployeeService>(null!, null!);
        _controller = new EmployeeController(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetEmployeeById_ShouldReturnOk()
    {
        _serviceMock.Setup(s => s.GetEmployeeByIdAsync(1))
            .ReturnsAsync(new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "joh.doe@test.com",
                Password = "HisPassword123",
                Job = new Job { Id = 1, Title = "Dev", Role = "Backend" },
                Department = new Department { Id = 1, Name = "IT" },
                Birthdate = DateTime.Now.AddYears(-35)
            });

        var result = await _controller.GetEmployeeById(1);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllEmployees_ShouldReturnNotFound_WhenEmpty()
    {
        _serviceMock.Setup(s => s.GetAllEmployeesAsync())
            .ReturnsAsync(new List<Employee>());

        var result = await _controller.GetAllEmployees();

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllEmployees_ShouldReturnOk_WhenExists()
    {
        _serviceMock.Setup(s => s.GetAllEmployeesAsync())
            .ReturnsAsync(new List<Employee> { new()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@gmail.com",
                    Password = "0720001002",
                    Job = new Job { Id = 1, Title = "Dev", Role = "Backend" },
                    Department = new Department { Id = 1, Name = "IT" },
                    Birthdate = DateTime.Now.AddYears(-35)
                }
            });

        var result = await _controller.GetAllEmployees();

        Assert.IsType<OkObjectResult>(result.Result);
    }
}