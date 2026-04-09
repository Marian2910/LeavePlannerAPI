using AutoMapper;
using FluentAssertions;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
// Aliases to avoid confusion
using EmployeeEntity = LeavePlanner.Infrastructure.Entities.Employee;
using PersonalEventEntity = LeavePlanner.Infrastructure.Entities.PersonalEvent;
using JobEntity = LeavePlanner.Infrastructure.Entities.Job;
using DepartmentEntity = LeavePlanner.Infrastructure.Entities.Department;

namespace LeavePlanner.Tests.Unit.Services;

public class PersonalEventServiceTests
{
    private readonly Mock<IPersonalEventRepository> _personalEventRepoMock = new();
    private readonly Mock<IEmployeeRepository> _employeeRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<PersonalEventService>> _loggerMock = new();

    private readonly PersonalEventService _service;

    public PersonalEventServiceTests()
    {
        _service = new PersonalEventService(
            _personalEventRepoMock.Object,
            _employeeRepoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    // ========================
    // 🔧 Helpers
    // ========================

    private static EmployeeEntity CreateValidEmployee(int remainingDays = 10)
    {
        return new EmployeeEntity
        {
            Id = 1,
            Job = new JobEntity { Id = 1, Title = "Dev" },
            Department = new DepartmentEntity { Id = 1, Name = "IT" },
            Birthdate = DateTime.UtcNow.AddYears(-25),
            RemainingLeaveDays = remainingDays
        };
    }

    private static PersonalEvent CreateValidPersonalEvent(int employeeId = 1, int days = 1)
    {
        return new PersonalEvent
        {
            EmployeeId = employeeId,
            Title = "Vacation",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(days)
        };
    }

    // ========================
    // ✅ AddEventAsync
    // ========================

    [Fact]
    public async Task AddEventAsync_ShouldThrow_WhenEventIsNull()
    {
        Func<Task> act = async () => await _service.AddEventAsync(null);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddEventAsync_ShouldThrow_WhenNotEnoughLeaveDays()
    {
        var personalEvent = CreateValidPersonalEvent(days: 10);
        var employee = CreateValidEmployee(remainingDays: 1);

        _employeeRepoMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(employee);

        Func<Task> act = async () => await _service.AddEventAsync(personalEvent);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task AddEventAsync_ShouldUpdateEmployeeAndAddEvent()
    {
        var personalEvent = CreateValidPersonalEvent(days: 1);
        var employee = CreateValidEmployee(remainingDays: 10);
        var entity = new PersonalEventEntity();

        _employeeRepoMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(employee);

        _mapperMock
            .Setup(x => x.Map<PersonalEventEntity>(personalEvent))
            .Returns(entity);

        await _service.AddEventAsync(personalEvent);

        _employeeRepoMock.Verify(
            x => x.UpdateEmployeeAsync(It.IsAny<EmployeeEntity>()),
            Times.Once);

        _personalEventRepoMock.Verify(
            x => x.AddPersonalEventAsync(It.IsAny<PersonalEventEntity>()),
            Times.Once);

        employee.RemainingLeaveDays.Should().BeLessThan(10);
    }

    // ========================
    // ✅ GetAllPersonalEventsAsync
    // ========================

    [Fact]
    public async Task GetAllPersonalEventsAsync_ShouldReturnMappedEvents()
    {
        var entities = new List<PersonalEventEntity> { new(), new() };
        var mapped = new List<PersonalEvent>
        {
            new() { Title = "Test1" },
            new() { Title = "Test2" }
        };
        _personalEventRepoMock
            .Setup(x => x.GetAllPersonalEventsAsync())
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<PersonalEvent>>(entities))
            .Returns(mapped);

        var result = await _service.GetAllPersonalEventsAsync();

        result.Should().BeEquivalentTo(mapped);
    }

    // ========================
    // ✅ GetPersonalEventByIdAsync
    // ========================

    [Fact]
    public async Task GetPersonalEventByIdAsync_ShouldThrow_WhenIdInvalid()
    {
        Func<Task> act = async () => await _service.GetPersonalEventByIdAsync(0);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task GetPersonalEventByIdAsync_ShouldThrow_WhenNotFound()
    {
        _personalEventRepoMock
            .Setup(x => x.GetPersonalEventByIdAsync(1))!
            .ReturnsAsync((PersonalEventEntity?)null);

        Func<Task> act = async () => await _service.GetPersonalEventByIdAsync(1);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetPersonalEventByIdAsync_ShouldReturnMappedEvent()
    {
        var entity = new PersonalEventEntity();
        var mapped = new PersonalEvent
        {
            Title = "Test"
        };
        
        _personalEventRepoMock
            .Setup(x => x.GetPersonalEventByIdAsync(1))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(x => x.Map<PersonalEvent>(entity))
            .Returns(mapped);

        var result = await _service.GetPersonalEventByIdAsync(1);

        result.Should().Be(mapped);
    }

    // ========================
    // ✅ GetPersonalEventsByEmployeeId
    // ========================

    [Fact]
    public async Task GetPersonalEventsByEmployeeId_ShouldThrow_WhenInvalidId()
    {
        Func<Task> act = async () => await _service.GetPersonalEventsByEmployeeId(0);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task GetPersonalEventsByEmployeeId_ShouldReturnMappedEvents()
    {
        var entities = new List<PersonalEventEntity> { new() };
        var mapped = new List<PersonalEvent>
        {
            new() { Title = "Test" }
        };
        _personalEventRepoMock
            .Setup(x => x.GetPersonalEventsByEmployeeIdAsync(1))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<PersonalEvent>>(entities))
            .Returns(mapped);

        var result = await _service.GetPersonalEventsByEmployeeId(1);

        result.Should().BeEquivalentTo(mapped);
    }
}
