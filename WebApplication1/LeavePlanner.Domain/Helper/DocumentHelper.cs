
using LeavePlanner.Infrastructure.Interfaces;

namespace LeavePlanner.Domain.Helper
{
    public abstract class DocumentHelper
    {
        public static async Task ValidDocumentExists(int customerId, int documentId, IDocumentRepository documentRepository)
        {
            await documentRepository.GetDocumentByCustomerIdAsync(customerId, documentId);
        }
    }
}