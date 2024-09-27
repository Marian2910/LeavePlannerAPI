using Castle.Core.Logging;
using Infrastructure.Configuration;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1.DocumentTest
{
    [TestFixture]
    public class DocumentRepositoryTests
    {
        private static DbContextOptions<ApplicationDBContext> dbContextOptions = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(databaseName: "DocumentsTestsDB")
            .Options;

        private DocumentRepository _documentRepository;
        private ApplicationDBContext _applicationDBContext;
        private Mock<ILogger<DocumentRepository>> _logger;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _applicationDBContext = new ApplicationDBContext(dbContextOptions);
            _applicationDBContext.Database.EnsureCreated();
            _logger = new Mock<ILogger<DocumentRepository>>();
            _documentRepository = new DocumentRepository(_applicationDBContext, _logger.Object);
            await SeedDatabase();
        }

        [Test]
        public async Task GetAllDocumentsForACustomer_ShouldReturnAllDocuments()
        {
            //act
            var result = await _documentRepository.GetDocumentsByCustomerIdAsync(1);

            //Assert
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetDocumnentForACustomer_ShouldReturnDocument()
        {
            //act
            var result = await _documentRepository.GetDocumentByCustomerIdAsync(1, 1);

            //Assert
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteDocumentForCustomer_ShouldDeleteDocument()
        {
            // Arrange
            var documents = await _documentRepository.GetDocumentsByCustomerIdAsync(1);
            var count = documents.Count();

            // Act
            await _documentRepository.DeleteDocumentAsync(1, 1);
            documents = await _documentRepository.GetDocumentsByCustomerIdAsync(1);

            // Assert
            Assert.That(documents.Count(), Is.EqualTo(count - 1));
        }

        private async Task SeedDatabase()
        {
            var customer1 = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "johndoe@example.com",
                PhoneNumber = "1234567890",
                Country = "USA",
                City = "New York",
                PostalCode = "10001",
                Street = "Main Street",
                Number = "10",
                Status = true,
                BillingType = "Prepaid",
                Tva = 10,
                Addition = "Customer Note 1",
                Date = DateTime.Now
            };

            var customer2 = new Customer
            {
                Id = 2,
                Name = "Jane Smith",
                Email = "janesmith@example.com",
                PhoneNumber = "0987654321",
                Country = "UK",
                City = "London",
                PostalCode = "SW1A",
                Street = "Queen Street",
                Number = "20",
                Status = true,
                BillingType = "Postpaid",
                Tva = 20,
                Addition = "Customer Note 2",
                Date = DateTime.Now
            };

            var documents = new List<Document>
            {
                new Document
                {
                    Id = 1,
                    Name = "Document1",
                    Type = "PDF",
                    Date = DateTime.Now,
                    File = new byte[] { },
                    Customer = customer1
                },
                new Document
                {
                    Id = 2,
                    Name = "Document2",
                    Type = "Word",
                    Date = DateTime.Now.AddDays(-1),
                    File = new byte[] { },
                    Customer = customer2
                },
                new Document
                {
                    Id = 3,
                    Name = "Document3",
                    Type = "Excel",
                    Date = DateTime.Now.AddDays(-2),
                    File = new byte[] { },
                    Customer = customer1
                },
                new Document
                {
                    Id = 4,
                    Name = "Document4",
                    Type = "Image",
                    Date = DateTime.Now.AddDays(-3),
                    File = new byte[] { },
                    Customer = customer2
                },
                new Document
                {
                    Id = 5,
                    Name = "Document5",
                    Type = "Text",
                    Date = DateTime.Now.AddDays(-4),
                    File = new byte[] { },
                    Customer = customer1
                }
            };

            await _applicationDBContext.Customers.AddAsync(customer1);
            await _applicationDBContext.Customers.AddAsync(customer2);
            await _applicationDBContext.Documents.AddRangeAsync(documents);
            await _applicationDBContext.SaveChangesAsync();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            _applicationDBContext.Dispose();
        }
    }
}
