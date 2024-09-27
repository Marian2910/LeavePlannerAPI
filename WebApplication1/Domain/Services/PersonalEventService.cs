using AutoMapper;
using Domain.Helper;
using Domain.Models;
using Infrastructure.Exceptions;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Domain.Services
{
    public class PersonalEventService
    {
        private readonly IPersonalEventRepository _personalEventRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PersonalEventService> _logger;

        public PersonalEventService(IPersonalEventRepository personalEventRepository, IEmployeeRepository employeeRepository, IMapper mapper, ILogger<PersonalEventService> logger)
        {
            _personalEventRepository = personalEventRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task AddEventAsync(PersonalEvent personalEvent)
        {
            _logger.LogInformation("AddEventAsync was called from PersonalEventService");
            var existingEmployee = await _employeeRepository.GetByIdAsync(personalEvent.EmployeeId) ?? throw new NullEntity("Employee not found.");

            int leaveDaysToSubstract = EventHelper.getWorkDaysBetweenDates(personalEvent.StartDate, personalEvent.EndDate);
            if (leaveDaysToSubstract > existingEmployee.RemainingLeaveDays)
            {
                throw new InvalidOperationException("Employee does not have enough leave days.");
            }
            existingEmployee.RemainingLeaveDays -= leaveDaysToSubstract;

            var personalEventMapped = _mapper.Map<Infrastructure.Entities.PersonalEvent>(personalEvent);
            personalEventMapped.Employee = existingEmployee;

            await _employeeRepository.UpdateEmployeeAsync(existingEmployee);
            await _personalEventRepository.AddPersonalEventAsync(personalEventMapped);
        }

        public async Task<IEnumerable<PersonalEvent>> GetAllPersonalEventsAsync()
        {
            _logger.LogInformation("GetAllPersonalEventsAsync was called from PersonalEventService");

            var personalEvents = await _personalEventRepository.GetAllPersonalEventsAsync();
            return _mapper.Map<IEnumerable<Domain.Models.PersonalEvent>>(personalEvents);
        }

        public async Task<PersonalEvent> GetPersonalEventByIdAsync(int id)
        {
            _logger.LogInformation("GetPersonalEventByIdAsync was called from PersonalEventService");

            var personalEvent = await _personalEventRepository.GetPersonalEventByIdAsync(id);
            return _mapper.Map<PersonalEvent>(personalEvent);
        }

        public async Task<IEnumerable<PersonalEvent>> GetPersonalEventsByEmployeeId(int employeeId)
        {
            _logger.LogInformation("GetPersonalEventsByEmployeeId was called from PersonalEventService");

            var personalEvents = await _personalEventRepository.GetPersonalEventsByEmployeeIdAsync(employeeId);
            return _mapper.Map<IEnumerable<Domain.Models.PersonalEvent>>(personalEvents);
        }
    }
}
