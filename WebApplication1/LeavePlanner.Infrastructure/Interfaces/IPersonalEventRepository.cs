using LeavePlanner.Infrastructure.Entities;

namespace LeavePlanner.Infrastructure.Interfaces
{
    public interface IPersonalEventRepository
    {
        Task AddPersonalEventAsync(PersonalEvent personalEvent);
        Task<IEnumerable<PersonalEvent>> GetAllPersonalEventsAsync();
        Task<PersonalEvent> GetPersonalEventByIdAsync(int id);
        Task<IEnumerable<PersonalEvent>> GetPersonalEventsByEmployeeIdAsync(int employeeId);
    }
}