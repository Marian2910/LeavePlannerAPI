using Infrastructure.Entities;

namespace Infrastructure.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
    }
}