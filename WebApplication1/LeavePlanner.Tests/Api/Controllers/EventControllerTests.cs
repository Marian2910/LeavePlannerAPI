using AutoMapper;
using Common.DTOs;
using LeavePlanner.Api.Controllers;
using LeavePlanner.Domain.Services;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using EventEntity = LeavePlanner.Infrastructure.Entities.Event;

namespace LeavePlanner.Tests.Api.Controllers;

public class EventControllerTests
{
    private readonly Mock<IEventRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly EventController _controller;

    public EventControllerTests()
    {
        var service = new EventService(
            _repositoryMock.Object,
            _mapperMock.Object,
            NullLogger<EventService>.Instance);

        _controller = new EventController(service, NullLogger<EventController>.Instance);
    }

    [Fact]
    public async Task GetAllEvents_ShouldReturnNotFound_WhenEmpty()
    {
        var entities = new List<EventEntity>();
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<EventDto>>(entities)).Returns(new List<EventDto>());

        var result = await _controller.GetAllEvents();

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllEvents_ShouldReturnOk()
    {
        var entities = new List<EventEntity> { new() };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<EventDto>>(entities))
            .Returns(new List<EventDto> { new TestEventDto { Title = "some event", StartDate = default } });

        var result = await _controller.GetAllEvents();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    private class TestEventDto : EventDto { }
}
