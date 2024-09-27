using Domain.Services;
using Infrastructure.Exceptions;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Helper
{
    public class DocumentHelper
    {
        public static async Task ValidDocumentExists<T>(int customerId, int documentId, IDocumentRepository documentRepository, ILogger<T> logger)
        {
            var document = await documentRepository.GetDocumentByCustomerIdAsync(customerId, documentId);
            if (document == null)
            {
                logger.LogError($"Could not find document with id {documentId} for customer with id {customerId}");
                throw new NullEntity($"Could not find document with id {documentId} for customer with id {customerId}");
            }
        }
    }
}
