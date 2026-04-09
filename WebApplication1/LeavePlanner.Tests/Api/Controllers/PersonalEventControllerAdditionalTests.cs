using AutoMapper;
using Common.DTOs;
using FluentAssertions;
using LeavePlanner.Api.Controllers;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Api.Controllers;

public class PersonalEventControllerAdditionalTests
{
    private readonly Mock<IPersonalEventRepository> _personalEventRepositoryMock = new();
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly PersonalEventController _controller;

    public PersonalEventControllerAdditionalTests()
    {
        var service = new PersonalEventService(
            _personalEventRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _mapperMock.Object,
            NullLogger<PersonalEventService>.Instance);

        _controller = new PersonalEventController(service, _mapperMock.Object, NullLogger<PersonalEventController>.Instance);
    }

    [Fact]
    public async Task GetPersonalEventById_ShouldReturnOk()
    {
        var entity = new LeavePlanner.Infrastructure.Entities.PersonalEvent { Id = 1, Title = "Vacation" };
        var model = new PersonalEvent { Id = 1, Title = "Vacation", EmployeeId = 1 };
        var dto = new PersonalEventDto { Title = "Vacation", StartDate = DateTime.Today, EmployeeId = 1 };

        _personalEventRepositoryMock.Setup(r => r.GetPersonalEventByIdAsync(1)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<PersonalEvent>(entity)).Returns(model);
        _mapperMock.Setup(m => m.Map<PersonalEventDto>(model)).Returns(dto);

        var result = await _controller.GetPersonalEventById(1);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPersonalEventsByEmployeeId_ShouldReturnOk_WhenEventsExist()
    {
        var entities = new List<LeavePlanner.Infrastructure.Entities.PersonalEvent> { new() { Title = "Vacation" } };
        var models = new List<PersonalEvent> { new() { Title = "Vacation", EmployeeId = 1 } };
        var dtos = new List<PersonalEventDto> { new() { Title = "Vacation", StartDate = DateTime.Today, EmployeeId = 1 } };

        _personalEventRepositoryMock.Setup(r => r.GetPersonalEventsByEmployeeIdAsync(1)).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<PersonalEvent>>(entities)).Returns(models);
        _mapperMock.Setup(m => m.Map<IEnumerable<PersonalEventDto>>(models)).Returns(dtos);

        var result = await _controller.GetPersonalEventsByEmployeeId(1);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPersonalEventsByEmployeeId_ShouldReturnNotFound_WhenEmpty()
    {
        _personalEventRepositoryMock.Setup(r => r.GetPersonalEventsByEmployeeIdAsync(1))
            .ReturnsAsync(new List<LeavePlanner.Infrastructure.Entities.PersonalEvent>());
        _mapperMock.Setup(m => m.Map<IEnumerable<PersonalEvent>>(It.IsAny<IEnumerable<LeavePlanner.Infrastructure.Entities.PersonalEvent>>()))
            .Returns(Array.Empty<PersonalEvent>());

        var result = await _controller.GetPersonalEventsByEmployeeId(1);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}
