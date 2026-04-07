using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using LeavePlanner.Domain.Services;
using LeavePlanner.Domain.Models;

namespace LeavePlanner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController(DocumentService documentService, ILogger<DocumentController> logger)
        : ControllerBase
    {
        private readonly DocumentService _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        private readonly ILogger<DocumentController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Constructor cleaned up to remove dependencies not used in the controller

        [HttpGet("{customerId}")]
        public async Task<ActionResult<Document>> GetDocumentsByCustomerById(int customerId)
        {
            _logger.LogInformation($"{nameof(GetDocumentsByCustomerById)} was called from {nameof(DocumentController)}");
            var document = await _documentService.GetDocumentsByCustomerIdAsync(customerId);
            return Ok(document);
        }

        [HttpDelete("{customerId}/{documentId}")]
        public async Task<ActionResult> DeleteDocument(int customerId, int documentId)
        {
            _logger.LogInformation($"{nameof(DeleteDocument)} was called from {nameof(DocumentController)}");
            await _documentService.DeleteDocument(customerId, documentId);
            _logger.LogInformation("Document with ID {DocumentId} for customer {CustomerId} was deleted.", documentId, customerId);
            return Ok(new { message = "Document deleted successfully." });
        }
    }
}