using LeavePlanner.Infrastructure.Entities;
using LeavePlanner.Infrastructure.Exceptions;
using LeavePlanner.Infrastructure.Repositories;
using LeavePlanner.Tests.Integration.Fixtures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Integration.Repositories;

public class EventRepositoryTests
{
    private readonly Mock<ILogger<EventRepository>> _loggerMock = new();

    private static Event CreateEvent(int id = 1)
    {
        return new Event
        {
            Id = id,
            Title = "Base Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1)
        };
    }

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

    private static PersonalEvent CreatePersonalEvent(Employee employee, int id = 2)
    {
        return new PersonalEvent
        {
            Id = id,
            Title = "Personal Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            EmployeeId = employee.Id,
            Employee = employee
        };
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyBaseEvents()
    {
        await using var context = DbContextFactory.CreateContext();

        var employee = CreateEmployee();
        context.Employees.Add(employee);

        context.Events.AddRange(
            CreateEvent(),
            CreateEvent(2),
            CreatePersonalEvent(employee, 3)
        );

        await context.SaveChangesAsync();

        var repo = new EventRepository(context, _loggerMock.Object);

        var result = await repo.GetAllAsync();

        var list = result.ToList();

        Assert.Equal(2, list.Count);
        Assert.All(list, e => Assert.IsType<Event>(e));
    }

    [Fact]
    public async Task GetAllAsync_ShouldThrow_WhenNoBaseEvents()
    {
        await using var context = DbContextFactory.CreateContext();

        var employee = CreateEmployee();
        context.Employees.Add(employee);

        context.Events.Add(CreatePersonalEvent(employee, 1));

        await context.SaveChangesAsync();

        var repo = new EventRepository(context, _loggerMock.Object);

        await Assert.ThrowsAsync<NullEntityException>(() =>
            repo.GetAllAsync());
    }
}