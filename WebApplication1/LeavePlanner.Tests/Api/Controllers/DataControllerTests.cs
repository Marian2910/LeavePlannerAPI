using FluentAssertions;
using LeavePlanner.Api.Controllers;
using LeavePlanner.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LeavePlanner.Tests.Api.Controllers;

public class DataControllerTests
{
    [Fact]
    public void CreateData_ShouldPopulateDatabase_WhenEmpty()
    {
        using var dbContext = CreateDbContext();
        var controller = new DataController(dbContext);

        var result = controller.CreateData();

        result.Should().BeOfType<OkObjectResult>();
        dbContext.Jobs.Should().HaveCount(4);
        dbContext.Departments.Should().HaveCount(2);
        dbContext.Employees.Should().HaveCount(4);
        dbContext.Customers.Should().HaveCount(5);
    }

    [Fact]
    public void CreateData_ShouldNotDuplicateSeedData_WhenCalledTwice()
    {
        using var dbContext = CreateDbContext();
        var controller = new DataController(dbContext);

        controller.CreateData();
        controller.CreateData();

        dbContext.Jobs.Should().HaveCount(4);
        dbContext.Departments.Should().HaveCount(2);
        dbContext.Employees.Should().HaveCount(4);
        dbContext.Customers.Should().HaveCount(5);
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
