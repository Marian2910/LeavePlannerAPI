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
    private readonly PersonalEventService _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ILogger<PersonalEventController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [HttpPost]
    public async Task<IActionResult> AddEvent([FromForm] PersonalEventDto eventDto)
    {
        _logger.LogInformation("Adding a new personal event.");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for adding a personal event.");
            return BadRequest(ModelState);
        }

        var personalEvent = _mapper.Map<PersonalEvent>(eventDto);
        await _eventService.AddEventAsync(personalEvent);

        _logger.LogInformation("Successfully added a personal event.");
        return Ok(new { message = "The event was added successfully!" });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonalEventDto>>> GetAllPersonalEvents()
    {
        _logger.LogInformation("Fetching all personal events.");

        var personalEvents = await _eventService.GetAllPersonalEventsAsync();

        if (!personalEvents.Any())
        {
            _logger.LogInformation("No personal events found.");
            return NotFound(new { message = "No personal events found." });
        }

        var personalEventsDto = _mapper.Map<IEnumerable<PersonalEventDto>>(personalEvents);

        _logger.LogInformation("Successfully fetched all personal events.");
        return Ok(personalEventsDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PersonalEventDto>> GetPersonalEventById(int id)
    {
        _logger.LogInformation("Fetching personal event with ID: {Id}", id);

        var personalEvent = await _eventService.GetPersonalEventByIdAsync(id);

        var personalEventDto = _mapper.Map<PersonalEventDto>(personalEvent);

        _logger.LogInformation("Successfully fetched personal event with ID: {Id}", id);
        return Ok(personalEventDto);
    }

    [HttpGet("getByEmployee/{employeeId}")]
    public async Task<ActionResult<IEnumerable<PersonalEventDto>>> GetPersonalEventsByEmployeeId(int employeeId)
    {
        _logger.LogInformation("Fetching personal events for Employee ID: {EmployeeId}", employeeId);

        var personalEvents = await _eventService.GetPersonalEventsByEmployeeId(employeeId);
        if (!personalEvents.Any())
        {
            _logger.LogWarning("No personal events found for Employee ID {EmployeeId}.", employeeId);
            return NotFound(new { message = "This employee doesn't have any personal events." });
        }

        var personalEventsDto = _mapper.Map<IEnumerable<PersonalEventDto>>(personalEvents);

        _logger.LogInformation("Successfully fetched personal events for Employee ID: {EmployeeId}", employeeId);
        return Ok(personalEventsDto);
    }
}