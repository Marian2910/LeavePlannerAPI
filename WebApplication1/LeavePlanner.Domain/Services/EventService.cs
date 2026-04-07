using AutoMapper;
using Common.DTOs;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace LeavePlanner.Domain.Services
{
    public class EventService(
        IEventRepository eventRepository,
        IMapper mapper,
        ILogger<EventService> logger)
    {
        
        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            logger.LogInformation("Fetching all events.");

            var eventEntities = await eventRepository.GetAllAsync();

            // Direct mapping to DTOs (avoid unnecessary intermediate mapping)
            return mapper.Map<IEnumerable<EventDto>>(eventEntities);
        }
    }
}
