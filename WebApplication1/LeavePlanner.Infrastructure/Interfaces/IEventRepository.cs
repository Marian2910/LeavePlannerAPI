using LeavePlanner.Infrastructure.Entities;

namespace LeavePlanner.Infrastructure.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
    }
}