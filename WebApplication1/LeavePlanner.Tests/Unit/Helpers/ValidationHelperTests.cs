using System.ComponentModel.DataAnnotations;
using Common.DTOs;
using FluentAssertions;
using LeavePlanner.Domain.Helper;
using LeavePlanner.Domain.Services;
using LeavePlanner.Infrastructure.Exceptions;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Unit.Helpers;

public class ValidationHelperTests
{
    [Fact]
    public async Task ValidCustomerExists_ShouldThrow_WhenIdIsInvalid()
    {
        var repositoryMock = new Mock<ICustomerRepository>();

        var act = () => ValidationHelper.ValidCustomerExists(
            0,
            repositoryMock.Object,
            NullLogger<CustomerService>.Instance);

        await act.Should().ThrowAsync<LessThanZeroNumbersException>();
    }

    [Fact]
    public async Task ValidCustomerExists_ShouldQueryRepository_WhenIdIsValid()
    {
        var repositoryMock = new Mock<ICustomerRepository>();
        repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new LeavePlanner.Infrastructure.Entities.Customer
            {
                Id = 1,
                Name = "Test",
                Email = "test@test.com"
            });

        await ValidationHelper.ValidCustomerExists(
            1,
            repositoryMock.Object,
            NullLogger<CustomerService>.Instance);

        repositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task ValidEmployeeExists_ShouldThrow_WhenIdIsInvalid()
    {
        var repositoryMock = new Mock<IEmployeeRepository>();

        var act = () => ValidationHelper.ValidEmployeeExists(
            0,
            repositoryMock.Object,
            NullLogger<EmployeeService>.Instance);

        await act.Should().ThrowAsync<LessThanZeroNumbersException>();
    }

    [Fact]
    public async Task ValidEmployeeExists_ShouldQueryRepository_WhenIdIsValid()
    {
        var repositoryMock = new Mock<IEmployeeRepository>();
        repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new LeavePlanner.Infrastructure.Entities.Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Password = "secret"
            });

        await ValidationHelper.ValidEmployeeExists(
            1,
            repositoryMock.Object,
            NullLogger<EmployeeService>.Instance);

        repositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task ValidPagination_ShouldThrow_WhenPageNumberIsInvalid()
    {
        var act = () => ValidationHelper.ValidPagination(0, 10, NullLogger<object>.Instance);

        await act.Should().ThrowAsync<LessThanZeroNumbersException>();
    }

    [Fact]
    public async Task ValidPagination_ShouldThrow_WhenPageSizeIsInvalid()
    {
        var act = () => ValidationHelper.ValidPagination(1, 0, NullLogger<object>.Instance);

        await act.Should().ThrowAsync<LessThanZeroNumbersException>();
    }

    [Fact]
    public async Task ValidPagination_ShouldComplete_WhenValuesAreValid()
    {
        var act = () => ValidationHelper.ValidPagination(1, 10, NullLogger<object>.Instance);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public void ValidateModel_ShouldReturnValidationErrors_ForInvalidAnnotatedModel()
    {
        var model = new CustomerDto
        {
            Name = "",
            Email = "not-an-email",
            PhoneNumber = "12",
            Country = "",
            City = "",
            PostalCode = "",
            Street = "",
            BillingType = "",
            Tva = 120,
            Date = default
        };

        var results = ValidationHelper.ValidateModel(model);

        results.Should().NotBeEmpty();
        results.Select(r => r.ErrorMessage).Should().Contain(message => message!.Contains("Email"));
    }

    [Fact]
    public void ValidateModel_ShouldIncludeIValidatableObjectErrors()
    {
        var results = ValidationHelper.ValidateModel(new ValidatableSample());

        results.Should().Contain(r => r.ErrorMessage == "Custom validation failure");
    }

    [Fact]
    public async Task ValidDocumentExists_ShouldThrow_WhenRepositoryIsNull()
    {
        var act = () => ValidationHelper.ValidDocumentExists(1, 2, null!, NullLogger.Instance);

        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("documentRepository");
    }

    [Fact]
    public async Task ValidDocumentExists_ShouldThrow_WhenLoggerIsNull()
    {
        var repositoryMock = new Mock<IDocumentRepository>();

        var act = () => ValidationHelper.ValidDocumentExists(1, 2, repositoryMock.Object, null!);

        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("logger");
    }

    [Fact]
    public async Task ValidDocumentExists_ShouldQueryRepository_WhenArgumentsAreValid()
    {
        var repositoryMock = new Mock<IDocumentRepository>();
        repositoryMock.Setup(r => r.GetDocumentByCustomerIdAsync(1, 2))
            .ReturnsAsync(new LeavePlanner.Infrastructure.Entities.Document());

        await ValidationHelper.ValidDocumentExists(1, 2, repositoryMock.Object, NullLogger.Instance);

        repositoryMock.Verify(r => r.GetDocumentByCustomerIdAsync(1, 2), Times.Once);
    }

    private sealed class ValidatableSample : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return new ValidationResult("Custom validation failure");
        }
    }
}
