using AutoMapper;
using LeavePlanner.Domain.Helper;
using LeavePlanner.Domain.Models;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using PersonalEventEntity = LeavePlanner.Infrastructure.Entities.PersonalEvent;

namespace LeavePlanner.Domain.Services
{
    public class PersonalEventService(
        IPersonalEventRepository personalEventRepository,
        IEmployeeRepository employeeRepository,
        IMapper mapper,
        ILogger<PersonalEventService> logger)
    {
        public async Task AddEventAsync(PersonalEvent? personalEvent)
        {
            logger.LogInformation(
                "Adding personal event for employee {EmployeeId}.",
                personalEvent?.EmployeeId);

            ArgumentNullException.ThrowIfNull(personalEvent);
            

            var existingEmployee = await employeeRepository.GetByIdAsync(personalEvent.EmployeeId);
            

            int leaveDaysToSubtract = EventHelper.GetWorkDaysBetweenDates(
                personalEvent.StartDate,
                personalEvent.EndDate);

            if (leaveDaysToSubtract > existingEmployee.RemainingLeaveDays)
            {
                throw new InvalidOperationException(
                    $"Employee {personalEvent.EmployeeId} does not have enough leave days.");
            }

            existingEmployee.RemainingLeaveDays -= leaveDaysToSubtract;

            var personalEventEntity = mapper.Map<PersonalEventEntity>(personalEvent);
            personalEventEntity.Employee = existingEmployee;

            await employeeRepository.UpdateEmployeeAsync(existingEmployee);
            await personalEventRepository.AddPersonalEventAsync(personalEventEntity);

            logger.LogInformation(
                "Personal event added for employee {EmployeeId}. Days subtracted: {Days}.",
                personalEvent.EmployeeId,
                leaveDaysToSubtract);
        }

        public async Task<IEnumerable<PersonalEvent>> GetAllPersonalEventsAsync()
        {
            logger.LogInformation("Fetching all personal events.");

            var personalEventEntities = await personalEventRepository.GetAllPersonalEventsAsync();

            return mapper.Map<IEnumerable<PersonalEvent>>(personalEventEntities);
        }

        public async Task<PersonalEvent> GetPersonalEventByIdAsync(int id)
        {
            logger.LogInformation("Fetching personal event with Id {EventId}.", id);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            var personalEventEntity = await personalEventRepository.GetPersonalEventByIdAsync(id);

            if (personalEventEntity == null)
            {
                throw new KeyNotFoundException($"Personal event with ID {id} not found.");
            }

            return mapper.Map<PersonalEvent>(personalEventEntity);
        }

        public async Task<IEnumerable<PersonalEvent>> GetPersonalEventsByEmployeeId(int employeeId)
        {
            logger.LogInformation(
                "Fetching personal events for employee {EmployeeId}.",
                employeeId);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(employeeId);

            var personalEventEntities = await personalEventRepository.GetPersonalEventsByEmployeeIdAsync(employeeId);

            return mapper.Map<IEnumerable<PersonalEvent>>(personalEventEntities);
        }
    }
}
