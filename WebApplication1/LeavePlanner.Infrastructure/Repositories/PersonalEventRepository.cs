using LeavePlanner.Infrastructure.Interfaces;
using LeavePlanner.Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LeavePlanner.Infrastructure.Configuration;
using PersonalEventEntity = LeavePlanner.Infrastructure.Entities.PersonalEvent;

namespace LeavePlanner.Infrastructure.Repositories
{
    public class PersonalEventRepository(ApplicationDbContext dbContext, ILogger<PersonalEventRepository> logger) : IPersonalEventRepository
    {
        public async Task AddPersonalEventAsync(PersonalEventEntity personalEvent)
        {
            logger.LogInformation(
                "Adding personal event for employee {EmployeeId}.",
                personalEvent.EmployeeId);

            ArgumentNullException.ThrowIfNull(personalEvent);

            await Validator.ValidEntity(personalEvent, logger);

            // Ensure EF does not try to insert/update Employee
            dbContext.Entry(personalEvent.Employee).State = EntityState.Unchanged;

            await dbContext.PersonalEvents.AddAsync(personalEvent);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<PersonalEventEntity>> GetAllPersonalEventsAsync()
        {
            logger.LogInformation("Fetching all personal events.");

            var personalEvents = await dbContext.PersonalEvents
                .AsNoTracking()
                .Include(e => e.Employee)
                .ToListAsync();

            await Validator.ValidEntities(personalEvents, logger);

            return personalEvents;
        }

        public async Task<PersonalEventEntity> GetPersonalEventByIdAsync(int id)
        {
            logger.LogInformation("Fetching personal event with Id {EventId}.", id);

            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var personalEvent = await dbContext.PersonalEvents
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (personalEvent == null)
            {
                logger.LogWarning("Personal event with Id {EventId} not found.", id);
            }

            await Validator.ValidEntity(personalEvent, logger);

            return personalEvent ?? throw new InvalidOperationException();
        }

        public async Task<IEnumerable<PersonalEventEntity>> GetPersonalEventsByEmployeeIdAsync(int employeeId)
        {
            logger.LogInformation(
                "Fetching personal events for employee {EmployeeId}.",
                employeeId);

            if (employeeId <= 0)
                throw new ArgumentOutOfRangeException(nameof(employeeId));

            var personalEvents = await dbContext.PersonalEvents
                .AsNoTracking()
                .Include(e => e.Employee)
                .Where(e => e.EmployeeId == employeeId) // FIX: avoid navigation property
                .ToListAsync();

            await Validator.ValidEntities(personalEvents, logger);

            return personalEvents;
        }
    }
}