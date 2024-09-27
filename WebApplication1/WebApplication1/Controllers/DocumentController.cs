using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Domain.Services;
using Common.DTOs;
using Domain.Models;

namespace ProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(DocumentService documentService, IMapper mapper, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{customerId}")]
        public async Task<ActionResult<Document>> GetDocumentsByCustomerById(int customerId)
        {
            _logger.LogInformation($"{nameof(GetDocumentsByCustomerById)} was called from {nameof(DocumentController)}");
            var document = await _documentService.GetDocumentsByCustomerIdAsync(customerId);
            if (document == null)
            {
                return NotFound($"The customer with Id {customerId} doesn't have any documents uploaded or doesn't exist.");
            }
            return Ok(document);
        }
        [HttpDelete("{customerId}/{documentId}")]
        public async Task<ActionResult<Document>> DeleteDocument(int customerId, int documentId)
        {
            _logger.LogInformation($"{nameof(DeleteDocument)} was called from {nameof(DocumentController)}");
            await _documentService.DeleteDocument(customerId, documentId);
            return Ok();
        }
    }
}
