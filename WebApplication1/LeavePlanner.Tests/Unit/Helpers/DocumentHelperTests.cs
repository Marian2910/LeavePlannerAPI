using FluentAssertions;
using LeavePlanner.Domain.Helper;
using LeavePlanner.Infrastructure.Interfaces;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Unit.Helpers;

public class DocumentHelperTests
{
    [Fact]
    public async Task ValidDocumentExists_ShouldQueryRepository()
    {
        var repositoryMock = new Mock<IDocumentRepository>();

        repositoryMock
            .Setup(r => r.GetDocumentByCustomerIdAsync(1, 2))
            .ReturnsAsync(new LeavePlanner.Infrastructure.Entities.Document());

        await DocumentHelper.ValidDocumentExists(1, 2, repositoryMock.Object);

        repositoryMock.Verify(r => r.GetDocumentByCustomerIdAsync(1, 2), Times.Once);
    }
}
