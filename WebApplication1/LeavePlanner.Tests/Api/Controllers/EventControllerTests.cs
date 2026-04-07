using AutoMapper;
using Common.DTOs;
using LeavePlanner.Api.Controllers;
using LeavePlanner.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Api.Controllers;

public class EventControllerTests
{
    private readonly Mock<EventService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<EventController>> _loggerMock = new();

    private readonly EventController _controller;

    public EventControllerTests()
    {
        _serviceMock = new Mock<EventService>(null!, null!, null!);
        _controller = new EventController(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllEvents_ShouldReturnNotFound_WhenEmpty()
    {
        _serviceMock.Setup(s => s.GetAllEventsAsync())
            .ReturnsAsync(new List<EventDto>());

        var result = await _controller.GetAllEvents();

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllEvents_ShouldReturnOk()
    {
        _serviceMock.Setup(s => s.GetAllEventsAsync())
            .ReturnsAsync(new List<EventDto> { new TestEventDto() });

        var result = await _controller.GetAllEvents();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    private class TestEventDto : EventDto { }
}