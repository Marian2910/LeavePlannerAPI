using AutoMapper;
using Common.DTOs;
using LeavePlanner.Api.Controllers;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using LeavePlanner.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using CustomerEntity = LeavePlanner.Infrastructure.Entities.Customer;

namespace LeavePlanner.Tests.Api.Controllers;

public class CustomerControllerTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        var service = new CustomerService(
            _repositoryMock.Object,
            _mapperMock.Object,
            NullLogger<CustomerService>.Instance);

        _controller = new CustomerController(service, NullLogger<CustomerController>.Instance, _mapperMock.Object);
    }

    private static Customer CreateCustomer()
    {
        return new Customer
        {
            Id = 1,
            Name = "Test",
            Email = "test@test.com",
            PhoneNumber = "123-456-7890",
            Country = "RO",
            City = "Cluj",
            PostalCode = "400000",
            Street = "Main",
            BillingType = "SRL",
            Date = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task GetAllCustomers_ShouldReturnOk()
    {
        var dto = new PagedResultDto<Customer> { Items = new List<Customer>(), TotalCount = 0 };
        var entities = new List<CustomerEntity>();

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.GetTotalCustomerCountAsync()).ReturnsAsync(0);
        _mapperMock.Setup(m => m.Map<IEnumerable<Customer>>(entities)).Returns(dto.Items);

        var result = await _controller.GetAllCustomers(1, 10);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetCustomerById_ShouldReturnOk()
    {
        var customer = CreateCustomer();
        var entity = new CustomerEntity
        {
            Id = 1,
            Name = customer.Name,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Country = customer.Country,
            City = customer.City,
            PostalCode = customer.PostalCode,
            Street = customer.Street,
            BillingType = customer.BillingType,
            Date = customer.Date
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<Customer>(entity)).Returns(customer);

        var result = await _controller.GetCustomerById(1);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task AddCustomer_ShouldReturnOk_WhenValid()
    {
        var dto = new CustomerDto
        {
            Name = "John",
            Email = "john.doe@test.com",
            PhoneNumber = "0722010200",
            Country = "Ro",
            City = "Bacau",
            PostalCode = "201002",
            Street = "Sperantei",
            BillingType = "-"
        };
        var customer = CreateCustomer();

        _mapperMock.Setup(m => m.Map<Customer>(dto)).Returns(customer);
        _mapperMock.Setup(m => m.Map<LeavePlanner.Infrastructure.Entities.Customer>(customer))
            .Returns(new CustomerEntity
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Country = customer.Country,
                City = customer.City,
                PostalCode = customer.PostalCode,
                Street = customer.Street,
                BillingType = customer.BillingType,
                Date = customer.Date
            });
        _repositoryMock.Setup(r => r.AddCustomerAsync(It.IsAny<CustomerEntity>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.AddCustomer(dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AddCustomer_ShouldReturnBadRequest_WhenInvalid()
    {
        _controller.ModelState.AddModelError("error", "invalid");

        var result = await _controller.AddCustomer(new CustomerDto
        {
            Name = "Istvan",
            Email = "Yodoe@test.com",
            PhoneNumber = "0722010200",
            Country = "Ro",
            City = "Timis",
            PostalCode = "202102",
            Street = "Independentei",
            BillingType = "-"
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteCustomer_ShouldReturnOk()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new CustomerEntity
            {
                Id = 1,
                Name = "Test",
                Email = "test@test.com"
            });
        _repositoryMock.Setup(r => r.DeleteCustomerAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteCustomer(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DeleteMultipleCustomers_ShouldReturnOk()
    {
        _repositoryMock.Setup(r => r.DeleteMultipleCustomers(It.IsAny<int[]>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.DeleteMultipleCustomers([1, 2]);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task SearchCustomersByName_ShouldReturnOk()
    {
        var entities = new List<LeavePlanner.Infrastructure.Entities.Customer>
        {
            new() { Id = 1, Name = "Test", Email = "test@test.com" }
        };
        var models = new List<Customer> { CreateCustomer() };

        _repositoryMock.Setup(r => r.SearchCustomersAsync("Test")).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<Customer>>(entities)).Returns(models);

        var result = await _controller.SearchCustomersByName("Test", 1, 10);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateCustomer_ShouldReturnOk_WhenValid()
    {
        var dto = new UpdateCustomerDto(1)
        {
            Name = "Updated",
            Email = "updated@test.com",
            PhoneNumber = "123-456-7890",
            Country = "RO",
            City = "Cluj",
            PostalCode = "400000",
            Street = "Main",
            BillingType = "SRL"
        };
        var customer = CreateCustomer();
        var entity = new LeavePlanner.Infrastructure.Entities.Customer
        {
            Id = 1,
            Name = customer.Name,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Country = customer.Country,
            City = customer.City,
            PostalCode = customer.PostalCode,
            Street = customer.Street,
            BillingType = customer.BillingType,
            Date = customer.Date
        };

        _mapperMock.Setup(m => m.Map<Customer>(dto)).Returns(customer);
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<LeavePlanner.Infrastructure.Entities.Customer>(customer)).Returns(entity);
        _repositoryMock.Setup(r => r.UpdateCustomerAsync(entity)).Returns(Task.CompletedTask);

        var result = await _controller.UpdateCustomer(dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AddDocumentToCustomer_ShouldReturnOk_WhenValid()
    {
        var customerEntity = new LeavePlanner.Infrastructure.Entities.Customer
        {
            Id = 1,
            Name = "Test",
            Email = "test@test.com"
        };
        var fileMock = new Mock<Microsoft.AspNetCore.Http.IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("test.pdf");
        fileMock.Setup(f => f.ContentType).Returns("application/pdf");
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns<Stream, CancellationToken>((stream, _) =>
            {
                var bytes = new byte[] { 1, 2, 3 };
                stream.Write(bytes, 0, bytes.Length);
                return Task.CompletedTask;
            });

        _repositoryMock.SetupSequence(r => r.GetByIdAsync(1))
            .ReturnsAsync(customerEntity)
            .ReturnsAsync(customerEntity);
        _repositoryMock.Setup(r => r.UpdateCustomerAsync(customerEntity)).Returns(Task.CompletedTask);

        var result = await _controller.AddDocumentToCustomer(1, new[] { fileMock.Object });

        Assert.IsType<OkObjectResult>(result);
    }
}
