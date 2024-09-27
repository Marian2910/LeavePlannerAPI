using AutoMapper;
using Domain.Services;
using Domain.Models;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1.DocumentTest
{
    public class DocumentsServiceTests
    {
        private DocumentService _documentService;
        private Mock<ICustomerRepository> _customerRepository;
        private Mock<IDocumentRepository> _documentRepository;
        private Mock<IMapper> _mapper;
        private Mock<ILogger<DocumentService>> _logger;


        [OneTimeSetUp]
        public void SetUp()
        {
            _customerRepository = new Mock<ICustomerRepository>();
            _documentRepository = new Mock<IDocumentRepository>();
            _mapper = new Mock<IMapper>();
            _logger = new Mock<ILogger<DocumentService>>();
            _documentService = new DocumentService(_documentRepository.Object, _customerRepository.Object, _mapper.Object, _logger.Object);
        }

        [Test]
        public async Task GetAllDocumentsForACustomer_ShouldReturnAllDocuments()
        {
            //arrange
            var customer1 = new Infrastructure.Entities.Customer
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

            var customer1Entity = new Customer
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

            var documents = new List<Infrastructure.Entities.Document>
            {
                new Infrastructure.Entities.Document
                {
                    Id = 1,
                    Name = "Document1",
                    Type = "PDF",
                    Date = DateTime.Now,
                    File = new byte[] { },
                    Customer = customer1
                },
                new Infrastructure.Entities.Document
                {
                    Id = 2,
                    Name = "Document2",
                    Type = "Word",
                    Date = DateTime.Now.AddDays(-1),
                    File = new byte[] { },
                    Customer = customer1
                },
                new Infrastructure.Entities.Document
                {
                    Id = 3,
                    Name = "Document3",
                    Type = "Excel",
                    Date = DateTime.Now.AddDays(-2),
                    File = new byte[] { },
                    Customer = customer1
                },
            };

            var documentsEntities = new List<Document>
            {
                new Document
                {
                    Id = 1,
                    Name = "Document1",
                    Type = "PDF",
                    Date = DateTime.Now,
                    File = new byte[] { },
                    Customer = customer1Entity
                },
                new Document
                {
                    Id = 2,
                    Name = "Document2",
                    Type = "Word",
                    Date = DateTime.Now.AddDays(-1),
                    File = new byte[] { },
                    Customer = customer1Entity
                },
                new Document
                {
                    Id = 3,
                    Name = "Document3",
                    Type = "Excel",
                    Date = DateTime.Now.AddDays(-2),
                    File = new byte[] { },
                    Customer = customer1Entity
                },

            };

            _documentRepository.Setup(repo => repo.GetDocumentsByCustomerIdAsync(customer1.Id)).ReturnsAsync(documents);

            _mapper.Setup(mapper => mapper.Map<IEnumerable<Document>>(documents)).Returns(documentsEntities);

            //act
            var result = await _documentService.GetDocumentsByCustomerIdAsync(customer1.Id);
            var count = result.Count();

            //assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task DeleteDocumentForCustomer_ShouldDeleteDocumentSuccessfully()
        {
            // Arrange
            var customer = new Infrastructure.Entities.Customer
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

            var document = new Infrastructure.Entities.Document
            {
                Id = 1,
                Name = "Document1",
                Type = "PDF",
                Date = DateTime.Now,
                File = new byte[] { },
                Customer = customer
            };

            _customerRepository.Setup(repo => repo.GetByIdAsync(customer.Id))
                .ReturnsAsync(customer);
            _documentRepository.Setup(repo => repo.GetDocumentByCustomerIdAsync(customer.Id, document.Id))
                .ReturnsAsync(document);
            _documentRepository.Setup(repo => repo.DeleteDocumentAsync(customer.Id, document.Id))
                .Returns(Task.CompletedTask);

            // Act
            await _documentService.DeleteDocument(customer.Id, document.Id);

            // Assert
            _documentRepository.Verify(repo => repo.DeleteDocumentAsync(customer.Id, document.Id), Times.Once);
        }
    }
}
