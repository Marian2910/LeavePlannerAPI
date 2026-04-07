using AutoMapper;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

using CustomerEntity = LeavePlanner.Infrastructure.Entities.Customer;
using DocumentEntity = LeavePlanner.Infrastructure.Entities.Document;

namespace LeavePlanner.Tests.Unit.Services;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<CustomerService>> _loggerMock = new();

    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _service = new CustomerService(
            _repoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    private static Customer CreateCustomer(int id = 1)
    {
        return new Customer
        {
            Id = id,
            Name = "Test Customer",
            Email = "test@test.com",
            PhoneNumber = "123-456-7890",
            Country = "Romania",
            City = "Cluj",
            PostalCode = "400000",
            Street = "Main Street",
            Number = "1",
            BillingType = "SRL",
            Tva = 19,
            Addition = "",
            Status = true,
            Documents = new List<Document>(),
            Date = DateTime.UtcNow
        };
    }

    private static CustomerEntity CreateCustomerEntity(int id = 1)
    {
        return new CustomerEntity
        {
            Id = id,
            Name = "Test Customer",
            Email = "test@test.com",
            PhoneNumber = "123456789",
            Country = "Romania",
            City = "Cluj",
            PostalCode = "400000",
            Street = "Main Street",
            Number = "1",
            BillingType = "SRL",
            Tva = 19,
            Addition = "",
            Status = true,
            Documents = new List<DocumentEntity>(),
            Date = DateTime.UtcNow
        };
    }
    
    [Fact]
    public async Task AddCustomer_ShouldCallRepository()
    {
        var customer = CreateCustomer();
        var entity = CreateCustomerEntity();

        _mapperMock
            .Setup(m => m.Map<CustomerEntity>(customer))
            .Returns(entity);

        await _service.AddCustomer(customer);

        _repoMock.Verify(r => r.AddCustomerAsync(entity), Times.Once);
    }
    
    [Fact]
    public async Task UpdateCustomer_ShouldCallRepository()
    {
        var customer = CreateCustomer();
        var entity = CreateCustomerEntity();

        _mapperMock
            .Setup(m => m.Map<CustomerEntity>(customer))
            .Returns(entity);

        _repoMock
            .Setup(r => r.GetByIdAsync(customer.Id))
            .ReturnsAsync(entity);

        await _service.UpdateCustomer(customer);

        _repoMock.Verify(r => r.UpdateCustomerAsync(entity), Times.Once);
    }
    
    [Fact]
    public async Task DeleteCustomer_ShouldCallRepository()
    {
        var id = 1;

        _repoMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(CreateCustomerEntity(id));

        await _service.DeleteCustomer(id);

        _repoMock.Verify(r => r.DeleteCustomerAsync(id), Times.Once);
    }
    
    [Fact]
    public async Task GetCustomerByIdAsync_ShouldReturnMappedCustomer()
    {
        var entity = CreateCustomerEntity();
        var model = CreateCustomer();

        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(m => m.Map<Customer>(entity))
            .Returns(model);

        var result = await _service.GetCustomerByIdAsync(1);

        Assert.Equal(model.Id, result.Id);
    }

    // ========================
    // ✅ GetCustomersAsync
    // ========================

    [Fact]
    public async Task GetCustomersAsync_ShouldReturnPagedResult()
    {
        var entities = new List<CustomerEntity>
        {
            CreateCustomerEntity(),
            CreateCustomerEntity(2)
        };

        var models = new List<Customer>
        {
            CreateCustomer(),
            CreateCustomer(2)
        };

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _repoMock.Setup(r => r.GetTotalCustomerCountAsync()).ReturnsAsync(2);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<Customer>>(entities))
            .Returns(models);

        var result = await _service.GetCustomersAsync(1, 10);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }
    
    [Fact]
    public async Task AddDocumentsToCustomer_ShouldAddValidDocument()
    {
        var customerEntity = CreateCustomerEntity();

        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(customerEntity);

        var fileMock = new Mock<IFormFile>();

        var content = "Test file content";
        var fileName = "test.pdf";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns("application/pdf");
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), CancellationToken.None))
            .Returns<Stream, CancellationToken>((s, _) =>
            {
                stream.CopyTo(s);
                return Task.CompletedTask;
            });

        await _service.AddDocumentsToCustomer(1, [fileMock.Object]);

        _repoMock.Verify(r => r.UpdateCustomerAsync(customerEntity), Times.Once);
        Assert.Single(customerEntity.Documents);
    }

    [Fact]
    public async Task AddDocumentsToCustomer_ShouldThrow_WhenInvalidFileType()
    {
        var customerEntity = CreateCustomerEntity();

        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(customerEntity);

        var fileMock = new Mock<IFormFile>();

        fileMock.Setup(f => f.FileName).Returns("test.txt");
        fileMock.Setup(f => f.ContentType).Returns("text/plain");
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddDocumentsToCustomer(1, [fileMock.Object]));
    }
    
    [Fact]
    public async Task DeleteMultipleCustomers_ShouldCallRepository()
    {
        var ids = new[] { 1, 2 };

        await _service.DeleteMultipleCustomers(ids);

        _repoMock.Verify(r => r.DeleteMultipleCustomers(ids), Times.Once);
    }
}