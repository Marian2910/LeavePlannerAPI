using AutoMapper;
using Common.DTOs;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace ProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly EventService _eventService;
        private readonly IMapper _mapper;
        private readonly ILogger<EventController> _logger;

        public EventController(EventService eventService, IMapper mapper, ILogger<EventController> logger)
        {
            _eventService = eventService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents()
        {
            _logger.LogInformation($"{nameof(GetAllEvents)} was called from {nameof(EventController)}");
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

    }
}
