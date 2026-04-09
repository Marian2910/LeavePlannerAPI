using Common.DTOs;
using LeavePlanner.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeavePlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController(EventService eventService, ILogger<EventController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents()
    {
        logger.LogInformation("Fetching all events.");

        var events = await eventService.GetAllEventsAsync();

        if (!events.Any())
        {
            logger.LogInformation("No events found.");
            return NotFound(new { message = "No events found." });
        }

        logger.LogInformation("Successfully retrieved all events.");
        return Ok(events);
    }
}
