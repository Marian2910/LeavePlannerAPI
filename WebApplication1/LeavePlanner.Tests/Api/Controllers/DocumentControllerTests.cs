using LeavePlanner.Api.Controllers;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using CustomerEntity = LeavePlanner.Infrastructure.Entities.Customer;
using DocumentEntity = LeavePlanner.Infrastructure.Entities.Document;

namespace LeavePlanner.Tests.Api.Controllers;

public class DocumentControllerTests
{
    private readonly Mock<IDocumentRepository> _documentRepositoryMock = new();
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<AutoMapper.IMapper> _mapperMock = new();

    private readonly DocumentController _controller;

    public DocumentControllerTests()
    {
        var service = new DocumentService(
            _documentRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _mapperMock.Object,
            NullLogger<DocumentService>.Instance);

        _controller = new DocumentController(service, NullLogger<DocumentController>.Instance);
    }

    [Fact]
    public async Task GetDocuments_ShouldReturnOk()
    {
        var customer = new CustomerEntity
        {
            Id = 1,
            Name = "Test",
            Email = "test@test.com"
        };
        var documents = new List<DocumentEntity>();

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
        _documentRepositoryMock.Setup(r => r.GetDocumentsByCustomerIdAsync(1)).ReturnsAsync(documents);
        _mapperMock.Setup(m => m.Map<IEnumerable<Document>>(documents)).Returns(new List<Document>());

        var result = await _controller.GetDocumentsByCustomerById(1);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteDocument_ShouldReturnOk()
    {
        var customer = new CustomerEntity
        {
            Id = 1,
            Name = "Test",
            Email = "test@test.com"
        };

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
        _documentRepositoryMock.Setup(r => r.GetDocumentByCustomerIdAsync(1, 1))
            .ReturnsAsync(new DocumentEntity
            {
                Id = 1,
                Name = "contract.pdf",
                Type = "application/pdf",
                File = Array.Empty<byte>(),
                Customer = customer
            });
        _documentRepositoryMock.Setup(r => r.DeleteDocumentAsync(1, 1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteDocument(1, 1);

        Assert.IsType<OkObjectResult>(result);
    }
}
