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
        [HttpGet("{customerId}")]
        [ProducesResponseType(typeof(Document), StatusCodes.Status200OK)]
        public async Task<ActionResult<Document>> GetDocumentsByCustomerById(int customerId)
        {
            logger.LogInformation("{MethodName} was called from {ControllerName}", nameof(GetDocumentsByCustomerById), nameof(DocumentController));
            var document = await documentService.GetDocumentsByCustomerIdAsync(customerId);
            return Ok(document);
        }

        [HttpDelete("{customerId}/{documentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteDocument(int customerId, int documentId)
        {
            logger.LogInformation("{MethodName} was called from {ControllerName}", nameof(DeleteDocument), nameof(DocumentController));
            await documentService.DeleteDocument(customerId, documentId);
            logger.LogInformation("Document with ID {DocumentId} for customer {CustomerId} was deleted.", documentId, customerId);
            return Ok(new { message = "Document deleted successfully." });
        }
    }
}
