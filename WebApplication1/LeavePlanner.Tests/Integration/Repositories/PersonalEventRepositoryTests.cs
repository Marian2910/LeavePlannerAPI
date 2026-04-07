using LeavePlanner.Infrastructure.Configuration;
using LeavePlanner.Infrastructure.Entities;
using LeavePlanner.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace LeavePlanner.Tests.Integration.Repositories
{
    [TestFixture]
    public class PersonalEventRepositoryTests
    {
        private static readonly DbContextOptions<ApplicationDbContext> DbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "PersonalEventDB")
            .Options;

        private PersonalEventRepository _personalEventRepository;
        private ApplicationDbContext _applicationDbContext;
        private ILogger<PersonalEventRepository> _logger;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _applicationDbContext = new ApplicationDbContext(DbContextOptions);
            _applicationDbContext.Database.EnsureCreated();
            _logger = new Mock<ILogger<PersonalEventRepository>>().Object;
            _personalEventRepository = new PersonalEventRepository(_applicationDbContext, _logger);
            await SeedDatabase();
        }

        [Test]
        public async Task AddPersonalEventsAsync()
        {
            //arrange
            var employee = new Employee
            {
                Id = 3,
                FirstName = "Maia",
                LastName = "Pop",
                Email = "lalalal@mail.com",
                Password = "password"
            };
            _applicationDbContext.Employees.Add(employee);
            await _applicationDbContext.SaveChangesAsync();
            var personalEvent = new PersonalEvent
            {
                Id = 4,
                Title = "Test",
                Description = "Test",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                Location = "Cluj",
                Employee = employee
            };
            //act
            await _personalEventRepository.AddPersonalEventAsync(personalEvent);
            await _applicationDbContext.SaveChangesAsync();

            //assert
            var addedPersonalEvent = await _applicationDbContext.PersonalEvents.Include(e => e.Employee).FirstOrDefaultAsync(e => e.Title == "Test");
            Assert.That(addedPersonalEvent?.Title, Is.EqualTo("Test"));
        }

        [Test]
        public async Task GetAllPersonalEvents()
        {
            //act 
            var result = await _personalEventRepository.GetAllPersonalEventsAsync();

            //assert
            Assert.That(result.Count(), Is.EqualTo(4));
        }

        [Test]
        public async Task GetPersonalEventById()
        {
            //act
            int personalEventId = 1;
            var personalEvent = await _personalEventRepository.GetPersonalEventByIdAsync(personalEventId);

            //assert
            Assert.That(personalEvent.Title, Is.EqualTo("Event 1"));

        }

        [Test]
        public async Task GetPersonalEventByEmployeeId()
        {
            //act
            int employeeId = 1;
            var result = await _personalEventRepository.GetPersonalEventsByEmployeeIdAsync(employeeId);

            //assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            _applicationDbContext.Dispose();
        }

        private async Task SeedDatabase()
        {
            var employee1 = new Employee
            {
                Id = 1,
                FirstName = "Maria",
                LastName = "Popa",
                Email = "mariaPopa@mail.com",
                Password = "password",
            };
            var employee2 = new Employee
            {
                Id = 2,
                FirstName = "Maria",
                LastName = "Popa",
                Email = "mariaPopa@mail.com",
                Password = "password",
            };

            var personalEvents = new List<PersonalEvent>
            {
                new PersonalEvent
                {
                    Id = 1,
                    Title = "Event 1",
                    Description = "Test",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Location = "Cluj",
                    Employee = employee1
                },
                new PersonalEvent
                {
                    Id = 2,
                    Title = "Event 2",
                    Description = "Test",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Location = "Cluj",
                    Employee = employee2
                },
                new PersonalEvent
                {
                    Id = 3,
                    Title = "Event 1",
                    Description = "Test",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Location = "Cluj",
                    Employee = employee1
                }
            };
            await _applicationDbContext.Employees.AddRangeAsync(employee1, employee2);
            await _applicationDbContext.PersonalEvents.AddRangeAsync(personalEvents);
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
