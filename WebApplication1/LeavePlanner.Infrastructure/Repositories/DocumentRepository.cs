using LeavePlanner.Infrastructure.Configuration;
using LeavePlanner.Infrastructure.Interfaces;
using LeavePlanner.Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using DocumentEntity = LeavePlanner.Infrastructure.Entities.Document;

namespace LeavePlanner.Infrastructure.Repositories
{
    public class DocumentRepository(ApplicationDbContext dbContext, ILogger<DocumentRepository> logger) : IDocumentRepository
    {
        public async Task DeleteDocumentAsync(int customerId, int documentId)
        {
            logger.LogInformation(
                "Deleting document {DocumentId} for customer {CustomerId}.",
                documentId, customerId);

            var document = await GetDocumentByCustomerIdAsync(customerId, documentId);

            dbContext.Documents.Remove(document);
            await dbContext.SaveChangesAsync(); // FIX: awaited
        }

        public async Task<DocumentEntity> GetDocumentByCustomerIdAsync(int customerId, int documentId)
        {
            logger.LogInformation(
                "Fetching document {DocumentId} for customer {CustomerId}.",
                documentId, customerId);

            if (customerId <= 0 || documentId <= 0)
                throw new ArgumentOutOfRangeException("customerId");

            var document = await dbContext.Documents
                .AsNoTracking()
                .FirstOrDefaultAsync(doc =>
                    doc.CustomerId == customerId &&
                    doc.Id == documentId);

            if (document == null)
            {
                logger.LogWarning(
                    "Document {DocumentId} for customer {CustomerId} not found.",
                    documentId, customerId);
            }

            await Validator.ValidEntity(document, logger);

            return document ?? throw new InvalidOperationException();
        }

        public async Task<IEnumerable<DocumentEntity>> GetDocumentsByCustomerIdAsync(int customerId)
        {
            logger.LogInformation(
                "Fetching documents for customer {CustomerId}.",
                customerId);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(customerId);

            var documents = await dbContext.Documents
                .AsNoTracking()
                .Where(doc => doc.CustomerId == customerId)
                .ToListAsync();

            await Validator.ValidEntities(documents, logger);

            return documents;
        }
    }
}