using AutoMapper;
using Common.DTOs;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Domain.Services
{
    public class EventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EventService> _logger;

        public EventService(IEventRepository eventRepository, IMapper mapper, ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            _logger.LogInformation("GetAllEventsAsync was called from EventService");

            var events = await _eventRepository.GetAllAsync();
            var mappedEvents = _mapper.Map<IEnumerable<Domain.Models.Event>>(events);

            return _mapper.Map<IEnumerable<Common.DTOs.EventDto>>(mappedEvents);
        }

    }

}
