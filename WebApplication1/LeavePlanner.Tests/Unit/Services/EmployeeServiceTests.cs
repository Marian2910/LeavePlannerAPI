using AutoMapper;
using FluentAssertions;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Unit.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<EmployeeService>> _loggerMock = new();

        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _employeeService = new EmployeeService(
                _employeeRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        // ----------------------------
        // Helpers (IMPORTANT)
        // ----------------------------

        private static Employee CreateValidEmployee(int id = 1) => new()
        {
            Id = id,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Password = "password",
            Job = new Job { Id = 1, Title = "Dev", Role = "Backend" },
            Department = new Department { Id = 1, Name = "IT" },
            Birthdate = DateTime.UtcNow.AddYears(-25)
        };

        // ----------------------------
        // GetAllEmployeesAsync
        // ----------------------------

        [Fact]
        public async Task GetAllEmployeesAsync_ShouldReturnMappedEmployees()
        {
            // Arrange
            var entities = new List<Infrastructure.Entities.Employee>
            {
                new() { Id = 1 },
                new() { Id = 2 }
            };

            var mapped = new List<Employee>
            {
                CreateValidEmployee(),
                CreateValidEmployee(2)
            };

            _employeeRepositoryMock
                .Setup(r => r.GetAllEmployeeAsync())
                .ReturnsAsync(entities);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<Employee>>(entities))
                .Returns(mapped);

            // Act
            var result = await _employeeService.GetAllEmployeesAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        // ----------------------------
        // GetEmployeeByIdAsync
        // ----------------------------

        [Fact]
        public async Task GetEmployeeByIdAsync_ShouldReturnEmployee_WhenExists()
        {
            // Arrange
            var entity = new Infrastructure.Entities.Employee { Id = 1 };
            var mapped = CreateValidEmployee();

            _employeeRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(entity);

            _mapperMock
                .Setup(m => m.Map<Employee>(entity))
                .Returns(mapped);

            // Act
            var result = await _employeeService.GetEmployeeByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ShouldThrow_WhenIdInvalid()
        {
            // Act
            var act = async () => await _employeeService.GetEmployeeByIdAsync(0);

            // Assert
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ShouldThrow_WhenEmployeeNotFound()
        {
            // Arrange
            _employeeRepositoryMock
                .Setup(r => r.GetByIdAsync(1))!
                .ReturnsAsync((Infrastructure.Entities.Employee?)null);

            // Act
            var act = async () => await _employeeService.GetEmployeeByIdAsync(1);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        // ----------------------------
        // UpdateLeaveDaysForEmployeeAsync
        // ----------------------------

        [Fact]
        public async Task UpdateLeaveDaysForEmployeeAsync_ShouldUpdate_WhenLessThanExpected()
        {
            // Arrange
            var employee = new Infrastructure.Entities.Employee
            {
                Id = 1,
                EmploymentDate = DateTime.UtcNow.AddYears(-2),
                AnnualLeaveDays = 5
            };

            _employeeRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(employee);

            _employeeRepositoryMock
                .Setup(r => r.UpdateEmployeeAsync(employee))
                .Returns(Task.CompletedTask);

            // Act
            await _employeeService.UpdateLeaveDaysForEmployeeAsync(1);

            // Assert
            _employeeRepositoryMock.Verify(
                r => r.UpdateEmployeeAsync(It.IsAny<Infrastructure.Entities.Employee>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateLeaveDaysForEmployeeAsync_ShouldNotUpdate_WhenAlreadyEnoughDays()
        {
            // Arrange
            var employee = new Infrastructure.Entities.Employee
            {
                Id = 1,
                EmploymentDate = DateTime.UtcNow.AddYears(-2),
                AnnualLeaveDays = 100
            };

            _employeeRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(employee);

            // Act
            await _employeeService.UpdateLeaveDaysForEmployeeAsync(1);

            // Assert
            _employeeRepositoryMock.Verify(
                r => r.UpdateEmployeeAsync(It.IsAny<Infrastructure.Entities.Employee>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateLeaveDaysForEmployeeAsync_ShouldThrow_WhenIdInvalid()
        {
            // Act
            var act = async () => await _employeeService.UpdateLeaveDaysForEmployeeAsync(0);

            // Assert
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task UpdateLeaveDaysForEmployeeAsync_ShouldThrow_WhenEmployeeNotFound()
        {
            _employeeRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Infrastructure.Entities.Employee)null!);

            var act = async () => await _employeeService.UpdateLeaveDaysForEmployeeAsync(1);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}
