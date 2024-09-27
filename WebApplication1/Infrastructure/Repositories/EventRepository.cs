using Infrastructure.Configuration;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<EventRepository> _logger;

        public EventRepository(ApplicationDBContext dBContext, ILogger<EventRepository> logger)
        {
            _dbContext = dBContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            _logger.LogInformation($"{nameof(GetAllAsync)} was called from {nameof(EventRepository)}");

            var events = await _dbContext.Events.Where(e => e.Discriminator == "Event").ToListAsync();
            await Validator.ValidEntities(events, _logger);

            return events;
        }
    }
}
