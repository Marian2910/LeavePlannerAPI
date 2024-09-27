using Infrastructure.Configuration;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class PersonalEventRepository : IPersonalEventRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<PersonalEventRepository> _logger;

        public PersonalEventRepository(ApplicationDBContext dbContext, ILogger<PersonalEventRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AddPersonalEventAsync(PersonalEvent personalEvent)
        {
            _logger.LogInformation($"{nameof(AddPersonalEventAsync)} was called from {nameof(PersonalEventRepository)}");

            await Validator.ValidEntity(personalEvent, _logger);

            if (personalEvent.Employee != null)
            {
                _dbContext.Entry(personalEvent.Employee).State = EntityState.Unchanged;
            }

            await _dbContext.PersonalEvents.AddAsync(personalEvent);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<PersonalEvent>> GetAllPersonalEventsAsync()
        {
            _logger.LogInformation($"{nameof(GetAllPersonalEventsAsync)} was called from {nameof(PersonalEventRepository)}");

            var personalEventsToBeValidated = await _dbContext.PersonalEvents
                                                              .Include(e => e.Employee)
                                                              .AsNoTracking()
                                                              .ToListAsync();
            await Validator.ValidEntities(personalEventsToBeValidated, _logger);
            
            return personalEventsToBeValidated;
        }
         
        public async Task<PersonalEvent> GetPersonalEventByIdAsync(int id)
        {
            _logger.LogInformation($"{nameof(GetPersonalEventByIdAsync)} was called from {nameof(PersonalEventRepository)}");

            var personalEventToBeValidated = await _dbContext.PersonalEvents
                                                             .Include(e => e.Employee)
                                                             .FirstOrDefaultAsync(c => c.Id == id);
            await Validator.ValidEntity(personalEventToBeValidated, _logger);

            return personalEventToBeValidated;
        }

        public async Task<IEnumerable<PersonalEvent>> GetPersonalEventsByEmployeeIdAsync(int employeeId)
        {
            _logger.LogInformation($"{nameof(GetPersonalEventsByEmployeeIdAsync)} was called from {nameof(PersonalEventRepository)}");

            var personalEventsToBeValidated = await _dbContext.PersonalEvents
                                                              .Include(e => e.Employee)
                                                              .AsNoTracking()
                                                              .Where(e => e.Employee.Id == employeeId)
                                                              .ToListAsync();
            await Validator.ValidEntities(personalEventsToBeValidated, _logger);

            return personalEventsToBeValidated;
        }
    }
}
