using Infrastructure.Entities;

namespace Infrastructure.Interfaces
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(int customerId);
        Task<Document> GetDocumentByCustomerIdAsync(int customerId, int documentId);
        Task DeleteDocumentAsync(int customerId, int documentId);
    }
}