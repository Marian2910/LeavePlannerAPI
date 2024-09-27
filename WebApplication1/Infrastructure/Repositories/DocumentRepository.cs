using Infrastructure.Configuration;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<DocumentRepository> _logger;

        public DocumentRepository(ApplicationDBContext dbContext, ILogger<DocumentRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task DeleteDocumentAsync(int customerId, int documentId)
        {
            _logger.LogInformation($"{nameof(DeleteDocumentAsync)} was called from {nameof(DocumentRepository)}");
        
            var document = await GetDocumentByCustomerIdAsync(customerId, documentId);

            if (document != null) 
            { 
                _dbContext.Remove(document);
                _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Document> GetDocumentByCustomerIdAsync(int customerId, int documentId)
        {
            _logger.LogInformation($"{nameof(GetDocumentByCustomerIdAsync)} was called from {nameof(DocumentRepository)}");
            
            var document = _dbContext.Documents.Where(doc => doc.Customer.Id == customerId).FirstOrDefault(doc => doc.Id == documentId);
            await Validator.ValidEntity<Document>(document, _logger);

            return document;
        }

        public async Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(int customerId)
        {
            _logger.LogInformation($"{nameof(GetDocumentsByCustomerIdAsync)} was called from {nameof(DocumentRepository)}");

            var documentsToBeValidated = await _dbContext.Documents
                                   .Where(doc => doc.Customer.Id == customerId)
                                   .AsNoTracking()
                                   .ToListAsync(); 
            await Validator.ValidEntities(documentsToBeValidated, _logger);

            return documentsToBeValidated;
        }
    }
}