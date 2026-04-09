using AutoMapper;
using Common.DTOs;
using LeavePlanner.Domain.Services;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

using EventEntity = LeavePlanner.Infrastructure.Entities.Event;

namespace LeavePlanner.Tests.Unit.Services;

public class EventServiceTests
{
    private readonly Mock<IEventRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<EventService>> _loggerMock = new();

    private readonly EventService _service;

    public EventServiceTests()
    {
        _service = new EventService(
            _repoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    private static EventEntity CreateEvent(int id = 1)
    {
        return new EventEntity
        {
            Id = id,
            Title = "Test Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1)
        };
    }

    private static TestEventDto CreateEventDto(int id = 1)
    {
        return new TestEventDto
        {
            Id = id,
            Title = "Test Event",
            StartDate = DateTime.UtcNow
        };
    }

    // helper concrete DTO (because EventDto is abstract)
    private class TestEventDto : EventDto { }

    // ========================
    // ✅ GetAllEventsAsync
    // ========================

    [Fact]
    public async Task GetAllEventsAsync_ShouldReturnMappedEvents()
    {
        var entities = new List<EventEntity>
        {
            CreateEvent(),
            CreateEvent(2)
        };

        var dtos = new List<EventDto>
        {
            CreateEventDto(),
            CreateEventDto(2)
        };

        _repoMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<EventDto>>(entities))
            .Returns(dtos);

        var result = await _service.GetAllEventsAsync();

        Assert.Equal(2, result.Count());

        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<EventDto>>(entities), Times.Once);
    }

    [Fact]
    public async Task GetAllEventsAsync_ShouldReturnEmpty_WhenNoEvents()
    {
        var entities = new List<EventEntity>();
        var dtos = new List<EventDto>();

        _repoMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<EventDto>>(entities))
            .Returns(dtos);

        var result = await _service.GetAllEventsAsync();

        Assert.Empty(result);
    }
}
