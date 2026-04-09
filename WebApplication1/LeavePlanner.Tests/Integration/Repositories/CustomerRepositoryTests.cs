using LeavePlanner.Infrastructure.Entities;
using LeavePlanner.Infrastructure.Exceptions;
using LeavePlanner.Infrastructure.Repositories;
using LeavePlanner.Tests.Integration.Fixtures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LeavePlanner.Tests.Integration.Repositories;

public class CustomerRepositoryTests
{
    private readonly Mock<ILogger<CustomerRepository>> _loggerMock = new();

    private static Customer CreateValidCustomer(int id = 1)
    {
        return new Customer
        {
            Id = id,
            Name = "Test Customer",
            Email = "test@test.com",
            PhoneNumber = "123456789",
            Country = "RO",
            City = "Cluj",
            PostalCode = "400000",
            Street = "Main",
            Number = "1",
            BillingType = "SRL",
            Tva = 19,
            Addition = "",
            Date = DateTime.UtcNow
        };
    }

    // ========================
    // ✅ AddCustomerAsync
    // ========================

    [Fact]
    public async Task AddCustomerAsync_ShouldAddCustomer()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new CustomerRepository(context, _loggerMock.Object);

        var customer = CreateValidCustomer();

        await repo.AddCustomerAsync(customer);

        Assert.Single(context.Customers);
    }

    // ========================
    // ✅ DeleteCustomerAsync
    // ========================

    [Fact]
    public async Task DeleteCustomerAsync_ShouldSoftDelete()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new CustomerRepository(context, _loggerMock.Object);

        var customer = CreateValidCustomer();
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        await repo.DeleteCustomerAsync(customer.Id);

        Assert.False(context.Customers.First().Status);
    }

    [Fact]
    public async Task DeleteCustomerAsync_ShouldThrow_WhenInvalidId()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new CustomerRepository(context, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            repo.DeleteCustomerAsync(0));
    }

    [Fact]
    public async Task DeleteCustomerAsync_ShouldDoNothing_WhenNotFound()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new CustomerRepository(context, _loggerMock.Object);

        await repo.DeleteCustomerAsync(999);

        Assert.Empty(context.Customers);
    }

    // ========================
    // ✅ GetAllAsync
    // ========================

    [Fact]
    public async Task GetAllAsync_ShouldReturnCustomers()
    {
        await using var context = DbContextFactory.CreateContext();
        context.Customers.AddRange(
            CreateValidCustomer(),
            CreateValidCustomer(2));
        await context.SaveChangesAsync();

        var repo = new CustomerRepository(context, _loggerMock.Object);

        var result = await repo.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    // ========================
    // ✅ GetByIdAsync
    // ========================

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCustomer()
    {
        await using var context = DbContextFactory.CreateContext();
        var customer = CreateValidCustomer();
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var repo = new CustomerRepository(context, _loggerMock.Object);

        var result = await repo.GetByIdAsync(customer.Id);

        Assert.Equal(customer.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenInvalidId()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new CustomerRepository(context, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            repo.GetByIdAsync(0));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new CustomerRepository(context, _loggerMock.Object);

        await Assert.ThrowsAsync<NullEntityException>(() =>
            repo.GetByIdAsync(999));
    }

    // ========================
    // ✅ UpdateCustomerAsync
    // ========================

    [Fact]
    public async Task UpdateCustomerAsync_ShouldUpdateCustomer()
    {
        await using var context = DbContextFactory.CreateContext();
        var customer = CreateValidCustomer();
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var repo = new CustomerRepository(context, _loggerMock.Object);

        customer.Name = "Updated";

        await repo.UpdateCustomerAsync(customer);

        Assert.Equal("Updated", context.Customers.First().Name);
    }
    
    [Fact]
    public async Task AddCustomerAsync_ShouldThrow_WhenNull()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new CustomerRepository(context, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(Act);
        return;

        Task Act() => repo.AddCustomerAsync(null!);
    }

    // ========================
    // ✅ SearchCustomersAsync
    // ========================

    [Fact]
    public async Task SearchCustomersAsync_ShouldReturnMatching()
    {
        await using var context = DbContextFactory.CreateContext();
        context.Customers.AddRange(
            CreateValidCustomer(),
            new Customer
            {
                Id = 2,
                Name = "Another",
                Email = "a@test.com",
                Date = DateTime.UtcNow
            });
        await context.SaveChangesAsync();

        var repo = new CustomerRepository(context, _loggerMock.Object);

        var result = await repo.SearchCustomersAsync("Test");

        Assert.Single(result);
    }

    [Fact]
    public async Task SearchCustomersAsync_ShouldReturnEmpty_WhenInvalidInput()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new CustomerRepository(context, _loggerMock.Object);

        var result = await repo.SearchCustomersAsync("");

        Assert.Empty(result);
    }

    // ========================
    // ✅ GetTotalCustomerCountAsync
    // ========================

    [Fact]
    public async Task GetTotalCustomerCountAsync_ShouldReturnCount()
    {
        await using var context = DbContextFactory.CreateContext();
        context.Customers.AddRange(
            CreateValidCustomer(),
            CreateValidCustomer(2));
        await context.SaveChangesAsync();

        var repo = new CustomerRepository(context, _loggerMock.Object);

        var count = await repo.GetTotalCustomerCountAsync();

        Assert.Equal(2, count);
    }

    // ========================
    // ✅ FilterCustomersByStatusAsync
    // ========================

    [Fact]
    public async Task FilterCustomersByStatusAsync_ShouldReturnFiltered()
    {
        await using var context = DbContextFactory.CreateContext();
        context.Customers.AddRange(
            CreateValidCustomer(),
            new Customer
            {
                Id = 2,
                Name = "Inactive",
                Email = "inactive@test.com",
                Status = false,
                Date = DateTime.UtcNow
            });
        await context.SaveChangesAsync();

        var repo = new CustomerRepository(context, _loggerMock.Object);

        var result = await repo.FilterCustomersByStatusAsync(true, 1, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task FilterCustomersByStatusAsync_ShouldThrow_WhenInvalidPaging()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new CustomerRepository(context, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            repo.FilterCustomersByStatusAsync(true, 0, 0));
    }

    // ========================
    // ✅ DeleteMultipleCustomers
    // ========================

    [Fact]
    public async Task DeleteMultipleCustomers_ShouldSoftDelete()
    {
        await using var context = DbContextFactory.CreateContext();
        context.Customers.AddRange(
            CreateValidCustomer(),
            CreateValidCustomer(2));
        await context.SaveChangesAsync();

        var repo = new CustomerRepository(context, _loggerMock.Object);

        await repo.DeleteMultipleCustomers([1, 2]);

        Assert.All(context.Customers, c => Assert.False(c.Status));
    }

    [Fact]
    public async Task DeleteMultipleCustomers_ShouldDoNothing_WhenNull()
    {
        await using var context = DbContextFactory.CreateContext();
        var repo = new CustomerRepository(context, _loggerMock.Object);

        await repo.DeleteMultipleCustomers(null);

        Assert.Empty(context.Customers);
    }
}
