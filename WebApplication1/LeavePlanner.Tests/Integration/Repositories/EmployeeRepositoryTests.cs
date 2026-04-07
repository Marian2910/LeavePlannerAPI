using LeavePlanner.Infrastructure.Entities;
using LeavePlanner.Infrastructure.Repositories;
using LeavePlanner.Tests.Integration.Fixtures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Integration.Repositories;

public class EmployeeRepositoryTests
{
    private readonly Mock<ILogger<EmployeeRepository>> _loggerMock = new();

    private static (Department, Job) CreateSharedDependencies()
    {
        var department = new Department { Id = 1, Name = "IT" };
        var job = new Job { Id = 1, Title = "Dev" };
        return (department, job);
    }

    private static Employee CreateEmployee(int id, Department department, Job job)
    {
        return new Employee
        {
            Id = id,
            FirstName = $"John{id}",
            LastName = "Doe",
            Email = $"john{id}@test.com",
            Password = "Password123!",
            JobId = job.Id,
            DepartmentId = department.Id,
            Job = job,
            Department = department,
            Birthdate = DateTime.UtcNow.AddYears(-25),
            EmploymentDate = DateTime.UtcNow,
            RemainingLeaveDays = 30,
            AnnualLeaveDays = 30
        };
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEmployee()
    {
        await using var context = DbContextFactory.CreateContext();

        var (department, job) = CreateSharedDependencies();
        context.Departments.Add(department);
        context.Jobs.Add(job);

        var employee = CreateEmployee(1, department, job);
        context.Employees.Add(employee);

        await context.SaveChangesAsync();

        var repo = new EmployeeRepository(context, _loggerMock.Object);

        var result = await repo.GetByIdAsync(1);

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenInvalidId()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new EmployeeRepository(context, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            repo.GetByIdAsync(0));
    }

    [Fact]
    public async Task GetAllEmployeeAsync_ShouldReturnEmployees()
    {
        await using var context = DbContextFactory.CreateContext();

        var (department, job) = CreateSharedDependencies();
        context.Departments.Add(department);
        context.Jobs.Add(job);

        var e1 = CreateEmployee(1, department, job);
        var e2 = CreateEmployee(2, department, job);

        context.Employees.AddRange(e1, e2);

        await context.SaveChangesAsync();

        var repo = new EmployeeRepository(context, _loggerMock.Object);

        var result = await repo.GetAllEmployeeAsync();

        var list = result.ToList();

        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldUpdateEmployee()
    {
        await using var context = DbContextFactory.CreateContext();

        var (department, job) = CreateSharedDependencies();
        context.Departments.Add(department);
        context.Jobs.Add(job);

        var employee = CreateEmployee(1, department, job);
        context.Employees.Add(employee);

        await context.SaveChangesAsync();

        var repo = new EmployeeRepository(context, _loggerMock.Object);

        employee.FirstName = "Updated";

        await repo.UpdateEmployeeAsync(employee);

        Assert.Equal("Updated", context.Employees.First().FirstName);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldThrow_WhenNull()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new EmployeeRepository(context, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            repo.UpdateEmployeeAsync(null));
    }
}