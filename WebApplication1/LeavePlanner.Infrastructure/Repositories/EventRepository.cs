using LeavePlanner.Infrastructure.Interfaces;
using LeavePlanner.Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LeavePlanner.Infrastructure.Configuration;
using EventEntity = LeavePlanner.Infrastructure.Entities.Event;

namespace LeavePlanner.Infrastructure.Repositories
{
    public class EventRepository(ApplicationDbContext dbContext, ILogger<EventRepository> logger) : IEventRepository
    {
        public async Task<IEnumerable<EventEntity>> GetAllAsync()
        {
            logger.LogInformation("Fetching all base events.");

            var events = await dbContext.Events
                .AsNoTracking()
                .Where(e => EF.Property<string>(e, "Discriminator") == "Event")
                .ToListAsync();

            await Validator.ValidEntities(events, logger);

            return events;
        }
    }
}