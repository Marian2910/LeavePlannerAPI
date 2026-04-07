using LeavePlanner.Infrastructure.Configuration;
using LeavePlanner.Infrastructure.Entities;
using LeavePlanner.Infrastructure.Exceptions;
using LeavePlanner.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Integration.Repositories
{
    public class DocumentRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private async Task SeedData(ApplicationDbContext context)
        {
            context.Documents.AddRange(new List<Document>
            {
                new Document { Id = 1, CustomerId = 1 },
                new Document { Id = 2, CustomerId = 1 },
                new Document { Id = 3, CustomerId = 2 }
            });

            await context.SaveChangesAsync();
        }

        // ========================
        // ✅ DeleteDocumentAsync
        // ========================

        [Fact]
        public async Task DeleteDocumentAsync_ShouldRemoveDocument_WhenExists()
        {
            var options = CreateOptions();

            // Seed
            using (var context = new ApplicationDbContext(options))
            {
                await SeedData(context);
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var logger = new Mock<ILogger<DocumentRepository>>();
                var repo = new DocumentRepository(context, logger.Object);

                await repo.DeleteDocumentAsync(1, 1);
            }

            // Assert (NEW context → avoids tracking issues)
            using (var context = new ApplicationDbContext(options))
            {
                var exists = await context.Documents.AnyAsync(d => d.Id == 1);
                Assert.False(exists);
            }
        }

        [Fact]
        public async Task DeleteDocumentAsync_ShouldThrow_WhenNotFound()
        {
            var options = CreateOptions();

            using var context = new ApplicationDbContext(options);
            var logger = new Mock<ILogger<DocumentRepository>>();
            var repo = new DocumentRepository(context, logger.Object);

            await Assert.ThrowsAsync<NullEntity>(() =>
                repo.DeleteDocumentAsync(999, 999));
        }

        // ========================
        // ✅ GetDocumentByCustomerIdAsync
        // ========================

        [Fact]
        public async Task GetDocumentByCustomerIdAsync_ShouldReturnDocument_WhenExists()
        {
            var options = CreateOptions();

            using (var context = new ApplicationDbContext(options))
            {
                await SeedData(context);
            }

            using (var context = new ApplicationDbContext(options))
            {
                var logger = new Mock<ILogger<DocumentRepository>>();
                var repo = new DocumentRepository(context, logger.Object);

                var result = await repo.GetDocumentByCustomerIdAsync(1, 1);

                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
            }
        }

        [Fact]
        public async Task GetDocumentByCustomerIdAsync_ShouldThrow_WhenInvalidIds()
        {
            var options = CreateOptions();

            using var context = new ApplicationDbContext(options);
            var logger = new Mock<ILogger<DocumentRepository>>();
            var repo = new DocumentRepository(context, logger.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                repo.GetDocumentByCustomerIdAsync(0, -1));
        }

        [Fact]
        public async Task GetDocumentByCustomerIdAsync_ShouldThrow_WhenNotFound()
        {
            var options = CreateOptions();

            using var context = new ApplicationDbContext(options);
            var logger = new Mock<ILogger<DocumentRepository>>();
            var repo = new DocumentRepository(context, logger.Object);

            await Assert.ThrowsAsync<NullEntity>(() =>
                repo.GetDocumentByCustomerIdAsync(1, 999));
        }

        // ========================
        // ✅ GetDocumentsByCustomerIdAsync
        // ========================

        [Fact]
        public async Task GetDocumentsByCustomerIdAsync_ShouldReturnDocuments()
        {
            var options = CreateOptions();

            using (var context = new ApplicationDbContext(options))
            {
                await SeedData(context);
            }

            using (var context = new ApplicationDbContext(options))
            {
                var logger = new Mock<ILogger<DocumentRepository>>();
                var repo = new DocumentRepository(context, logger.Object);

                var result = await repo.GetDocumentsByCustomerIdAsync(1);

                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetDocumentsByCustomerIdAsync_ShouldThrow_WhenInvalidCustomerId()
        {
            var options = CreateOptions();

            using var context = new ApplicationDbContext(options);
            var logger = new Mock<ILogger<DocumentRepository>>();
            var repo = new DocumentRepository(context, logger.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                repo.GetDocumentsByCustomerIdAsync(0));
        }
    }
}