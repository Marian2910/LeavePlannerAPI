using AutoMapper;
using Common.DTOs;
using LeavePlanner.Api.Controllers;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Api.Controllers;

public class PersonalEventControllerTests
{
    private readonly Mock<PersonalEventService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<PersonalEventController>> _loggerMock = new();

    private readonly PersonalEventController _controller;

    public PersonalEventControllerTests()
    {
        _serviceMock = new Mock<PersonalEventService>(null!, null!, null!, null!);
        _controller = new PersonalEventController(_serviceMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task AddEvent_ShouldReturnOk_WhenValid()
    {
        var dto = new PersonalEventDto
        {
            Title = "Some event",
            StartDate = DateTime.Now.AddDays(1),
        };
        var entity = new PersonalEvent
        {
            Title = "Some event"
        };

        _mapperMock.Setup(m => m.Map<PersonalEvent>(dto)).Returns(entity);

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
        _serviceMock.Setup(s => s.GetAllPersonalEventsAsync())
            .ReturnsAsync(new List<PersonalEvent>());

        var result = await _controller.GetAllPersonalEvents();

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllPersonalEvents_ShouldReturnOk()
    {
        _serviceMock.Setup(s => s.GetAllPersonalEventsAsync())
            .ReturnsAsync(new List<PersonalEvent> { new()
                {
                    Title = "Some event"
                }
            });

        _mapperMock.Setup(m => m.Map<IEnumerable<PersonalEventDto>>(It.IsAny<IEnumerable<PersonalEvent>>()))
            .Returns(new List<PersonalEventDto>());

        var result = await _controller.GetAllPersonalEvents();

        Assert.IsType<OkObjectResult>(result.Result);
    }
}