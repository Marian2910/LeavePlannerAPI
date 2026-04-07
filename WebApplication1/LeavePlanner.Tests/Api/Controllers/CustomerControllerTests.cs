using AutoMapper;
using Common.DTOs;
using LeavePlanner.Api.Controllers;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Api.Controllers;

public class CustomerControllerTests
{
    private readonly Mock<CustomerService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<CustomerController>> _loggerMock = new();

    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        _serviceMock = new Mock<CustomerService>(null!, null!, null!);
        _controller = new CustomerController(_serviceMock.Object, _loggerMock.Object, _mapperMock.Object);
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

        _serviceMock.Setup(s => s.GetCustomersAsync(1, 10, null, null, null, null))
            .ReturnsAsync(dto);

        var result = await _controller.GetAllCustomers(1, 10);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetCustomerById_ShouldReturnOk()
    {
        var customer = CreateCustomer();

        _serviceMock.Setup(s => s.GetCustomerByIdAsync(1))
            .ReturnsAsync(customer);

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
        var result = await _controller.DeleteCustomer(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DeleteMultipleCustomers_ShouldReturnOk()
    {
        var result = await _controller.DeleteMultipleCustomers(new[] { 1, 2 });

        Assert.IsType<OkObjectResult>(result);
    }
}