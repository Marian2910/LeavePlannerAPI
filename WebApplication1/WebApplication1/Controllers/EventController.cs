using AutoMapper;
using Common.DTOs;
using LeavePlanner.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeavePlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController(EventService eventService, IMapper mapper, ILogger<EventController> logger) : ControllerBase
{
    private readonly EventService _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
    private readonly ILogger<EventController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents()
    {
        _logger.LogInformation("Fetching all events.");

        var events = await _eventService.GetAllEventsAsync();

        if (!events.Any())
        {
            _logger.LogInformation("No events found.");
            return NotFound(new { message = "No events found." });
        }

        _logger.LogInformation("Successfully retrieved all events.");
        return Ok(events);
    }
}