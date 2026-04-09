using AutoMapper;
using LeavePlanner.Api.Controllers;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using JobEntity = LeavePlanner.Infrastructure.Entities.Job;
using DepartmentEntity = LeavePlanner.Infrastructure.Entities.Department;
using EmployeeEntity = LeavePlanner.Infrastructure.Entities.Employee;

namespace LeavePlanner.Tests.Api.Controllers;

public class EmployeeControllerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly EmployeeController _controller;

    public EmployeeControllerTests()
    {
        var service = new EmployeeService(
            _repositoryMock.Object,
            _mapperMock.Object,
            NullLogger<EmployeeService>.Instance);

        _controller = new EmployeeController(service, NullLogger<EmployeeController>.Instance);
    }

    [Fact]
    public async Task GetEmployeeById_ShouldReturnOk()
    {
        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "joh.doe@test.com",
            Password = "HisPassword123",
            Job = new Job { Id = 1, Title = "Dev", Role = "Backend" },
            Department = new Department { Id = 1, Name = "IT" },
            Birthdate = DateTime.UtcNow.AddYears(-35)
        };
        var entity = new EmployeeEntity
        {
            Id = 1,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Password = employee.Password,
            Job = new JobEntity { Id = 1, Title = "Dev" },
            Department = new DepartmentEntity { Id = 1, Name = "IT" },
            Birthdate = employee.Birthdate
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<Employee>(entity)).Returns(employee);

        var result = await _controller.GetEmployeeById(1);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllEmployees_ShouldReturnNotFound_WhenEmpty()
    {
        var entities = new List<EmployeeEntity>();
        _repositoryMock.Setup(r => r.GetAllEmployeeAsync()).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<Employee>>(entities)).Returns(new List<Employee>());

        var result = await _controller.GetAllEmployees();

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllEmployees_ShouldReturnOk_WhenExists()
    {
        var employees = new List<Employee> { new()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@gmail.com",
                Password = "0720001002",
                Job = new Job { Id = 1, Title = "Dev", Role = "Backend" },
                Department = new Department { Id = 1, Name = "IT" },
                Birthdate = DateTime.UtcNow.AddYears(-35)
            }
        };
        var entities = new List<EmployeeEntity> { new()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@gmail.com",
                Password = "0720001002",
                Job = new JobEntity { Id = 1, Title = "Dev" },
                Department = new DepartmentEntity { Id = 1, Name = "IT" },
                Birthdate = DateTime.UtcNow.AddYears(-35)
            }
        };

        _repositoryMock.Setup(r => r.GetAllEmployeeAsync()).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<Employee>>(entities)).Returns(employees);

        var result = await _controller.GetAllEmployees();

        Assert.IsType<OkObjectResult>(result.Result);
    }
}
