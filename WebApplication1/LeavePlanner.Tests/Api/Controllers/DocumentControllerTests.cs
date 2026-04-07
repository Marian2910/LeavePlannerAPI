using LeavePlanner.Api.Controllers;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Api.Controllers;

public class DocumentControllerTests
{
    private readonly Mock<DocumentService> _serviceMock;
    private readonly Mock<ILogger<DocumentController>> _loggerMock = new();

    private readonly DocumentController _controller;

    public DocumentControllerTests()
    {
        _serviceMock = new Mock<DocumentService>(null!, null!);
        _controller = new DocumentController(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetDocuments_ShouldReturnOk()
    {
        _serviceMock.Setup(s => s.GetDocumentsByCustomerIdAsync(1))
            .ReturnsAsync(new List<Document>());

        var result = await _controller.GetDocumentsByCustomerById(1);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteDocument_ShouldReturnOk()
    {
        var result = await _controller.DeleteDocument(1, 1);

        Assert.IsType<OkObjectResult>(result);
    }
}