using AutoMapper;
using Domain.Helper;
using Domain.Models;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Domain.Services
{
    public class DocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(IDocumentRepository documentRepository,
                               ICustomerRepository customerRepository,
                               IMapper mapper,
                               ILogger<DocumentService> logger)
        {
            _documentRepository = documentRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(int customerId)
        {
            _logger.LogInformation("GetDocumentsByCustomerIdAsync was called from DocumentService");

            var documents = await _documentRepository.GetDocumentsByCustomerIdAsync(customerId);

            return _mapper.Map<IEnumerable<Document>>(documents);
        }

        public async Task DeleteDocument(int customerId, int documentId)
        {
            _logger.LogInformation($"{nameof(DeleteDocument)} was called from {nameof(DocumentService)}");

            await ValidationHelper.ValidCustomerExists(customerId, _customerRepository, _logger);
            await DocumentHelper.ValidDocumentExists(customerId, documentId, _documentRepository, _logger);

            await _documentRepository.DeleteDocumentAsync(customerId, documentId);

        }
    }
}
