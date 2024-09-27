using Infrastructure.Entities;

namespace Infrastructure.Interfaces
{
    public interface IPersonalEventRepository
    {
        Task AddPersonalEventAsync(PersonalEvent personalEvent);
        Task<IEnumerable<PersonalEvent>> GetAllPersonalEventsAsync();
        Task<PersonalEvent> GetPersonalEventByIdAsync(int id);
        Task<IEnumerable<PersonalEvent>> GetPersonalEventsByEmployeeIdAsync(int employeeId);
    }
}