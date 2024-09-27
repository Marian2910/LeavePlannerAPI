using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Domain.Services;
using Common.DTOs;
using Domain.Models;

namespace ProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonalEventController : ControllerBase
    {
        private readonly PersonalEventService _eventService;
        private readonly IMapper _mapper;
        private readonly ILogger<PersonalEventController> _logger;

        public PersonalEventController(PersonalEventService eventService, IMapper mapper, ILogger<PersonalEventController> logger)
        {
            _eventService = eventService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddEvent([FromForm] PersonalEventDto eventDto)
        {
            _logger.LogInformation($"{nameof(AddEvent)} was called from {nameof(PersonalEventController)}");
            if (ModelState.IsValid)
            {
                var eventt = _mapper.Map<PersonalEvent>(eventDto);
                await _eventService.AddEventAsync(eventt);
                return Ok("The event was added successfully!");
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonalEventDto>>> GetAllPersonalEvents()
        {
            _logger.LogInformation($"{nameof(GetAllPersonalEvents)} was called from {nameof(PersonalEventController)}");
            var personalEvents = await _eventService.GetAllPersonalEventsAsync();
            var personalEventsDto = _mapper.Map<IEnumerable<PersonalEventDto>>(personalEvents);
            return Ok(personalEventsDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PersonalEventDto>> GetPersonalEventById(int id)
        {
            _logger.LogInformation($"{nameof(GetPersonalEventById)} was called from {nameof(PersonalEventController)}");
            var personalEvent = await _eventService.GetPersonalEventByIdAsync(id);
            if (personalEvent == null)
            {
                return NotFound($"The customer with the {id} was not found");
            }
            var personalEventsDto = _mapper.Map<PersonalEventDto>(personalEvent);
            return Ok(personalEventsDto);
        }

        [HttpGet("getByEmployee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<PersonalEventDto>>> GetPersonalEventsByEmployeeId(int employeeId)
        {
            _logger.LogInformation($"{nameof(GetPersonalEventsByEmployeeId)} was called from {nameof(PersonalEventController)}");
            var personalEvent = await _eventService.GetPersonalEventsByEmployeeId(employeeId);
            if(personalEvent == null)
            {
                return NotFound("The employee doesn't have a personal event added");
            }
            var personaEventDto = _mapper.Map<IEnumerable<PersonalEventDto>>(personalEvent);
            return Ok(personaEventDto);
        }
    }
}
