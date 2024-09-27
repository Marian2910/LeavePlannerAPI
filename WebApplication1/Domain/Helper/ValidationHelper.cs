using Domain.Services;
using Infrastructure.Exceptions;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Domain.Helper
{
    public static class ValidationHelper
    {
        public static async Task ValidCustomerExists<T>(int customerId, ICustomerRepository _customerRepository, ILogger<T> logger)
        {
            if (customerId <= 0)
            {
                logger.LogError("Customer id must be greater than zero");
                throw new LessThanZeroNumbers("Customer id must be greater than zero");
            }

            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                logger.LogError($"Could not find customer with id {customerId}");
                throw new NullEntity($"Could not find customer with id {customerId}.");
            }
        }

        public static async Task ValidEmployeeExists(int employeeId, IEmployeeRepository _employeeRepository, ILogger<EmployeeService> logger)
        {
            if (employeeId <= 0)
            {
                logger.LogError("Employee id must be greater than zero.");
                throw new LessThanZeroNumbers("Employee id must be greater than zero.");
            }

            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                logger.LogError($"Could not find employee with id {employeeId}");
                throw new NullEntity($"Could not find employee with id {employeeId}.");
            }
        }

        public static async Task ValidPagination<T>(int pageNumber, int pageSize, ILogger<T> logger)
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
    }
}
