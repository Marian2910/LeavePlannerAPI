using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using LeavePlanner.Domain.Services;
using Common.DTOs;
using LeavePlanner.Domain.Models;

namespace LeavePlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonalEventController(
    PersonalEventService eventService, 
    IMapper mapper, 
    ILogger<PersonalEventController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddEvent([FromForm] PersonalEventDto eventDto)
    {
        logger.LogInformation("Adding a new personal event.");

        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for adding a personal event.");
            return BadRequest(ModelState);
        }

        var personalEvent = mapper.Map<PersonalEvent>(eventDto);
        await eventService.AddEventAsync(personalEvent);

        logger.LogInformation("Successfully added a personal event.");
        return Ok(new { message = "The event was added successfully!" });
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PersonalEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PersonalEventDto>>> GetAllPersonalEvents()
    {
        logger.LogInformation("Fetching all personal events.");

        var personalEvents = await eventService.GetAllPersonalEventsAsync();

        if (!personalEvents.Any())
        {
            logger.LogInformation("No personal events found.");
            return NotFound(new { message = "No personal events found." });
        }

        var personalEventsDto = mapper.Map<IEnumerable<PersonalEventDto>>(personalEvents);

        logger.LogInformation("Successfully fetched all personal events.");
        return Ok(personalEventsDto);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PersonalEventDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PersonalEventDto>> GetPersonalEventById(int id)
    {
        logger.LogInformation("Fetching personal event with ID: {Id}", id);

        var personalEvent = await eventService.GetPersonalEventByIdAsync(id);

        var personalEventDto = mapper.Map<PersonalEventDto>(personalEvent);

        logger.LogInformation("Successfully fetched personal event with ID: {Id}", id);
        return Ok(personalEventDto);
    }

    [HttpGet("getByEmployee/{employeeId}")]
    [ProducesResponseType(typeof(IEnumerable<PersonalEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PersonalEventDto>>> GetPersonalEventsByEmployeeId(int employeeId)
    {
        logger.LogInformation("Fetching personal events for Employee ID: {EmployeeId}", employeeId);

        var personalEvents = await eventService.GetPersonalEventsByEmployeeId(employeeId);
        if (!personalEvents.Any())
        {
            logger.LogWarning("No personal events found for Employee ID {EmployeeId}.", employeeId);
            return NotFound(new { message = "This employee doesn't have any personal events." });
        }

        var personalEventsDto = mapper.Map<IEnumerable<PersonalEventDto>>(personalEvents);

        logger.LogInformation("Successfully fetched personal events for Employee ID: {EmployeeId}", employeeId);
        return Ok(personalEventsDto);
    }
}
