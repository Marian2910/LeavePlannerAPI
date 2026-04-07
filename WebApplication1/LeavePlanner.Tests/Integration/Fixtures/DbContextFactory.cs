using LeavePlanner.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace LeavePlanner.Tests.Integration.Fixtures;

public static class DbContextFactory
{
    public static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // 🔥 unique DB per test
            .Options;

        var context = new ApplicationDbContext(options);

        return context;
    }

    public static ApplicationDbContext CreateContextWithData()
    {
        var context = CreateContext();

        Seed(context);

        return context;
    }

    private static void Seed(ApplicationDbContext context)
    {
        context.Documents.AddRange(
            new Infrastructure.Entities.Document { Id = 1, CustomerId = 1 },
            new Infrastructure.Entities.Document { Id = 2, CustomerId = 1 },
            new Infrastructure.Entities.Document { Id = 3, CustomerId = 2 }
        );

        context.SaveChanges();
    }
}