using AutoMapper;
using LeavePlanner.Domain.Helper;
using LeavePlanner.Domain.Models;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace LeavePlanner.Domain.Services
{
    public class DocumentService(
        IDocumentRepository documentRepository,
        ICustomerRepository customerRepository,
        IMapper mapper,
        ILogger<DocumentService> logger)
    {
        public async Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(int customerId)
        {
            logger.LogInformation("{MethodName} invoked for Customer ID: {CustomerId}", nameof(GetDocumentsByCustomerIdAsync), customerId);

            await ValidationHelper.ValidCustomerExists(customerId, customerRepository, logger);

            var documentEntities = await documentRepository.GetDocumentsByCustomerIdAsync(customerId);

            logger.LogInformation("Retrieved {DocumentCount} documents for Customer ID: {CustomerId}", documentEntities.Count(), customerId);

            return mapper.Map<IEnumerable<Document>>(documentEntities);
        }

        public async Task DeleteDocument(int customerId, int documentId)
        {
            logger.LogInformation("{MethodName} invoked for Customer ID: {CustomerId}, Document ID: {DocumentId}", nameof(DeleteDocument), customerId, documentId);

            await ValidationHelper.ValidCustomerExists(customerId, customerRepository, logger);
            await ValidationHelper.ValidDocumentExists(customerId, documentId, documentRepository, logger);

            await documentRepository.DeleteDocumentAsync(customerId, documentId);

            logger.LogInformation("Document ID: {DocumentId} successfully deleted for Customer ID: {CustomerId}", documentId, customerId);
        }
    }
}