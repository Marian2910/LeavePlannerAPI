using LeavePlanner.Infrastructure.Entities;
using LeavePlanner.Infrastructure.Exceptions;
using LeavePlanner.Infrastructure.Repositories;
using LeavePlanner.Tests.Integration.Fixtures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Integration.Repositories;

public class PersonalEventRepositoryTests
{
    private readonly Mock<ILogger<PersonalEventRepository>> _loggerMock = new();

    private static Employee CreateEmployee(int id = 1)
    {
        return new Employee
        {
            Id = id,
            FirstName = "John",
            LastName = "Doe",
            Email = $"john{id}@test.com",
            Password = "Password123!",
            JobId = 1,
            DepartmentId = 1,
            Birthdate = DateTime.UtcNow.AddYears(-25),
            EmploymentDate = DateTime.UtcNow,
            RemainingLeaveDays = 30,
            AnnualLeaveDays = 30
        };
    }

    private static PersonalEvent CreatePersonalEvent(int id = 1, int employeeId = 1)
    {
        return new PersonalEvent
        {
            Id = id,
            Title = "Vacation",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            EmployeeId = employeeId
        };
    }

    [Fact]
    public async Task AddPersonalEventAsync_ShouldAddEvent()
    {
        await using var context = DbContextFactory.CreateContext();

        var employee = CreateEmployee();
        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        var repo = new PersonalEventRepository(context, _loggerMock.Object);

        var personalEvent = CreatePersonalEvent(employeeId: employee.Id);
        personalEvent.Employee = employee;

        await repo.AddPersonalEventAsync(personalEvent);

        Assert.Single(context.PersonalEvents);
    }

    [Fact]
    public async Task AddPersonalEventAsync_ShouldThrow_WhenNull()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new PersonalEventRepository(context, _loggerMock.Object);

        await Assert.ThrowsAsync<NullReferenceException>(() =>
            repo.AddPersonalEventAsync(null!));
    }

    [Fact]
    public async Task GetAllPersonalEventsAsync_ShouldReturnEvents()
    {
        await using var context = DbContextFactory.CreateContext();

        var employee = CreateEmployee();
        context.Employees.Add(employee);

        var e1 = CreatePersonalEvent(1, employee.Id);
        var e2 = CreatePersonalEvent(2, employee.Id);

        e1.Employee = employee;
        e2.Employee = employee;

        context.PersonalEvents.AddRange(e1, e2);

        await context.SaveChangesAsync();

        var repo = new PersonalEventRepository(context, _loggerMock.Object);

        var result = await repo.GetAllPersonalEventsAsync();

        var list = result.ToList();

        Assert.Equal(2, list.Count);
        Assert.All(list, e => Assert.NotNull(e.Employee));
    }

    [Fact]
    public async Task GetPersonalEventByIdAsync_ShouldReturnEvent()
    {
        await using var context = DbContextFactory.CreateContext();

        var employee = CreateEmployee();
        context.Employees.Add(employee);

        var personalEvent = CreatePersonalEvent(1, employee.Id);
        personalEvent.Employee = employee;

        context.PersonalEvents.Add(personalEvent);

        await context.SaveChangesAsync();

        var repo = new PersonalEventRepository(context, _loggerMock.Object);

        var result = await repo.GetPersonalEventByIdAsync(1);

        Assert.Equal(1, result.Id);
        Assert.NotNull(result.Employee);
    }

    [Fact]
    public async Task GetPersonalEventByIdAsync_ShouldThrow_WhenInvalidId()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new PersonalEventRepository(context, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            repo.GetPersonalEventByIdAsync(0));
    }

    [Fact]
    public async Task GetPersonalEventByIdAsync_ShouldThrow_WhenNotFound()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new PersonalEventRepository(context, _loggerMock.Object);

        await Assert.ThrowsAsync<NullEntity>(() =>
            repo.GetPersonalEventByIdAsync(999));
    }

    [Fact]
    public async Task GetPersonalEventsByEmployeeIdAsync_ShouldReturnEvents()
    {
        await using var context = DbContextFactory.CreateContext();

        var employee = CreateEmployee(1);
        var otherEmployee = CreateEmployee(2);

        context.Employees.AddRange(employee, otherEmployee);

        var e1 = CreatePersonalEvent(1, 1);
        var e2 = CreatePersonalEvent(2, 1);
        var e3 = CreatePersonalEvent(3, 2);

        e1.Employee = employee;
        e2.Employee = employee;
        e3.Employee = otherEmployee;

        context.PersonalEvents.AddRange(e1, e2, e3);

        await context.SaveChangesAsync();

        var repo = new PersonalEventRepository(context, _loggerMock.Object);

        var result = await repo.GetPersonalEventsByEmployeeIdAsync(1);

        var list = result.ToList();

        Assert.Equal(2, list.Count);
        Assert.All(list, e => Assert.Equal(1, e.EmployeeId));
    }

    [Fact]
    public async Task GetPersonalEventsByEmployeeIdAsync_ShouldThrow_WhenInvalidId()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new PersonalEventRepository(context, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            repo.GetPersonalEventsByEmployeeIdAsync(0));
    }
}