using AutoMapper;
using Common.DTOs;
using LeavePlanner.Api.Controllers;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using PersonalEventEntity = LeavePlanner.Infrastructure.Entities.PersonalEvent;
using EmployeeEntity = LeavePlanner.Infrastructure.Entities.Employee;

namespace LeavePlanner.Tests.Api.Controllers;

public class PersonalEventControllerTests
{
    private readonly Mock<IPersonalEventRepository> _personalEventRepositoryMock = new();
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly PersonalEventController _controller;

    public PersonalEventControllerTests()
    {
        var service = new PersonalEventService(
            _personalEventRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _mapperMock.Object,
            NullLogger<PersonalEventService>.Instance);

        _controller = new PersonalEventController(service, _mapperMock.Object, NullLogger<PersonalEventController>.Instance);
    }

    [Fact]
    public async Task AddEvent_ShouldReturnOk_WhenValid()
    {
        var dto = new PersonalEventDto
        {
            Title = "Some event",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(1),
            EmployeeId = 1
        };
        var entity = new PersonalEvent
        {
            Title = "Some event",
            EmployeeId = 1,
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(1)
        };

        _mapperMock.Setup(m => m.Map<PersonalEvent>(dto)).Returns(entity);
        _mapperMock.Setup(m => m.Map<PersonalEventEntity>(entity))
            .Returns(new PersonalEventEntity
            {
                Title = entity.Title,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate
            });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new EmployeeEntity
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Password = "secret",
            RemainingLeaveDays = 10,
            EmploymentDate = DateTime.Today.AddYears(-2)
        });
        _employeeRepositoryMock.Setup(r => r.UpdateEmployeeAsync(It.IsAny<EmployeeEntity>()))
            .Returns(Task.CompletedTask);
        _personalEventRepositoryMock.Setup(r => r.AddPersonalEventAsync(It.IsAny<PersonalEventEntity>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.AddEvent(dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AddEvent_ShouldReturnBadRequest_WhenInvalid()
    {
        _controller.ModelState.AddModelError("err", "invalid");

        var result = await _controller.AddEvent(new PersonalEventDto
        {
            Title = "Some other event",
            StartDate = default
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAllPersonalEvents_ShouldReturnNotFound_WhenEmpty()
    {
        var entities = new List<PersonalEventEntity>();
        _personalEventRepositoryMock.Setup(r => r.GetAllPersonalEventsAsync()).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<PersonalEvent>>(entities)).Returns(new List<PersonalEvent>());

        var result = await _controller.GetAllPersonalEvents();

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllPersonalEvents_ShouldReturnOk()
    {
        var entities = new List<PersonalEventEntity> { new() { Title = "Some event" } };
        var models = new List<PersonalEvent> { new() { Title = "Some event", EmployeeId = 1 } };

        _personalEventRepositoryMock.Setup(r => r.GetAllPersonalEventsAsync()).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<PersonalEvent>>(entities)).Returns(models);

        _mapperMock.Setup(m => m.Map<IEnumerable<PersonalEventDto>>(models))
            .Returns(new List<PersonalEventDto>());

        var result = await _controller.GetAllPersonalEvents();

        Assert.IsType<OkObjectResult>(result.Result);
    }
}
