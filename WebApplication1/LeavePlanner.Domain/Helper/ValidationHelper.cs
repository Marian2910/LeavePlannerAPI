using LeavePlanner.Infrastructure.Exceptions;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using LeavePlanner.Domain.Services;

namespace LeavePlanner.Domain.Helper
{
    public static class ValidationHelper
    {
        public static async Task ValidCustomerExists<T>(int customerId, ICustomerRepository customerRepository, ILogger<T> logger)
        {
            if (customerId <= 0)
            {
                logger.LogError("Customer id must be greater than zero");
                throw new LessThanZeroNumbers("Customer id must be greater than zero");
            }

            await customerRepository.GetByIdAsync(customerId);
        }

        public static async Task ValidEmployeeExists(int employeeId, IEmployeeRepository employeeRepository, ILogger<EmployeeService> logger)
        {
            if (employeeId <= 0)
            {
                logger.LogError("Employee id must be greater than zero.");
                throw new LessThanZeroNumbers("Employee id must be greater than zero.");
            }

            await employeeRepository.GetByIdAsync(employeeId);
        }

        public static Task ValidPagination<T>(int pageNumber, int pageSize, ILogger<T> logger)
        {
            if (pageNumber <= 0)
            {
                logger.LogError("Page number must be greater than zero");
                throw new LessThanZeroNumbers("Page number must be greater than zero");
            }

            if (pageSize <= 0)
            {
                logger.LogError("Page size must be greater than zero");
                throw new LessThanZeroNumbers("Page size must be greater than zero");
            }

            return Task.CompletedTask;
        }

        public static IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();

            var validationContext = new ValidationContext(model);

            Validator.TryValidateObject(model, validationContext, results, true);

            if (model is IValidatableObject validatableModel)
            {
                results.AddRange(validatableModel.Validate(validationContext));
            }

            return results;
        }

        public static async Task ValidDocumentExists(
            int customerId, 
            int documentId, 
            IDocumentRepository documentRepository, 
            ILogger logger)
        {
            if (documentRepository == null) throw new ArgumentNullException(nameof(documentRepository));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            await documentRepository.GetDocumentByCustomerIdAsync(customerId, documentId);

            logger.LogInformation("Validated existence of Document ID: {DocumentId} for Customer ID: {CustomerId}", documentId, customerId);
        }
    }
}
