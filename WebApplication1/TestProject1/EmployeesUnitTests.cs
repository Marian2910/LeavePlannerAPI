using AutoMapper;
using Domain.MappingProfiles;
using Domain.Services;
using Infrastructure.Configuration;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class EmployeesUnitTests
    {
        private Domain.Models.Employee employee;
        private ApplicationDBContext dbContext;
        private EmployeeRepository employeeRepository;
        private ILogger<EmployeeRepository> loggerRepository;
        private IMapper mapper;
        private EmployeeService employeeService;
        private ILogger<EmployeeService> loggerService;

        [SetUp]
        public void Setup()
        {
            employee = CreateTestEmployee(
                "Ionel",
                "Pop",
                "ionel.pop@rtbsolutions.com",
                "password123",
                new Domain.Models.Job() { Role = "Admin", Title = "Finance Manager" },
                new Domain.Models.Department() { Name = "Finance" },
                Array.Empty<Domain.Models.PersonalEvent>(),
                new DateTime(1984, 06, 15),
                new DateTime(2019, 01, 15)
            );

            var options = new DbContextOptionsBuilder<ApplicationDBContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            dbContext = new ApplicationDBContext(options);

            loggerRepository = new Mock<ILogger<EmployeeRepository>>().Object;
            loggerService = new Mock<ILogger<EmployeeService>>().Object;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfiles>();
            });

            mapper = config.CreateMapper();

            employeeRepository = new EmployeeRepository(dbContext, loggerRepository);
            employeeService = new EmployeeService(employeeRepository, mapper, loggerService);

            dbContext.Employees.Add(mapper.Map<Infrastructure.Entities.Employee>(employee));
            dbContext.SaveChanges();
            dbContext.ChangeTracker.Clear();
        }

        [TearDown]
        public void Cleanup()
        {
            dbContext.Dispose();
        }

        private Domain.Models.Employee CreateTestEmployee(string firstName, string lastName, string email, string password, Domain.Models.Job job, Domain.Models.Department department, Domain.Models.PersonalEvent[] personalEvents, DateTime birthDate, DateTime employmentDate)
        {
            return new Domain.Models.Employee
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                Job = job,
                Department = department,
                PersonalEvents = personalEvents,
                Birthdate = birthDate,
                EmploymentDate = employmentDate
            };
        }

        private async Task<Infrastructure.Entities.Employee> GetByIdAsync(int id)
        {
            return await dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);
        }

        [Test]
        public async Task UpdateEmployeeAsync_ShouldUpdateEmployee()
        {
            var employeeToUpdate = await GetByIdAsync(1);
            Assert.That(employeeToUpdate, Is.Not.Null);

            employeeToUpdate.LastName = "Popa";

            await employeeRepository.UpdateEmployeeAsync(employeeToUpdate);

            var updatedEmployee = await dbContext.Employees.FindAsync(employeeToUpdate.Id);

            Assert.That(updatedEmployee, Is.Not.Null);
            Assert.That(updatedEmployee.LastName, Is.EqualTo("Popa"));
        }

        [Test]
        public async Task GetEmployeeByIdAsync_ShouldReturnEmployee()
        {
            var employee = await employeeService.GetEmployeeByIdAsync(1);

            Assert.That(employee, Is.Not.Null);
            Assert.That("ionel.pop@rtbsolutions.com", Is.EqualTo(employee.Email));
        }
    }
}
