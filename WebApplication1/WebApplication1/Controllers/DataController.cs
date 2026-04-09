using LeavePlanner.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using LeavePlanner.Infrastructure.Entities;

namespace LeavePlanner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController(ApplicationDbContext dbContext) : ControllerBase
    {
        private const string FinanceDepartment = "Finance";
        private const string ItTechDepartment = "IT/Tech";
        private const string NotApplicable = "N/A";

        private static DateTime CreateUtcDate(int year, int month, int day) =>
            new(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        
        [HttpPost("createData")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult CreateData()
        {
            if (!dbContext.Jobs.Any())
            {
                var jobs = new[]
                {
                    new Job { Title = "Finance Manager" },
                    new Job { Title = "Front-End Developer" },
                    new Job { Title = "Back-End Developer" },
                    new Job { Title = "Full-Stack Developer" }
                };
                dbContext.Jobs.AddRange(jobs);
                dbContext.SaveChanges();
            }

            if (!dbContext.Departments.Any())
            {
                var departments = new[]
                {
                    new Department { Name = FinanceDepartment },
                    new Department { Name = ItTechDepartment }
                };
                dbContext.Departments.AddRange(departments);
                dbContext.SaveChanges();
            }

            // Retrieve jobs and departments only once
            var jobsByTitle = dbContext.Jobs.ToDictionary(j => j.Title);
            var departmentsByName = dbContext.Departments.ToDictionary(d => d.Name);

            if (!dbContext.Set<Employee>().Any())
            {
                var employees = new[]
                {
                    new Employee {
                        Job = jobsByTitle["Finance Manager"], Department = departmentsByName[FinanceDepartment],
                        Birthdate = CreateUtcDate(1995, 10, 22), RemainingLeaveDays = 30, EmploymentDate = CreateUtcDate(2019, 5, 1), AnnualLeaveDays = 31
                    },
                    new Employee {
                        Job = jobsByTitle["Front-End Developer"], Department = departmentsByName[ItTechDepartment],
                        Birthdate = CreateUtcDate(1995, 10, 22), RemainingLeaveDays = 28, EmploymentDate = CreateUtcDate(2021, 10, 15), AnnualLeaveDays = 29
                    },
                    new Employee {
                        Job = jobsByTitle["Back-End Developer"], Department = departmentsByName[ItTechDepartment],
                        Birthdate = CreateUtcDate(1995, 10, 22), RemainingLeaveDays = 25, EmploymentDate = CreateUtcDate(2023, 10, 1), AnnualLeaveDays = 27
                    },
                    new Employee {
                        Job = jobsByTitle["Full-Stack Developer"], Department = departmentsByName[ItTechDepartment],
                        Birthdate = CreateUtcDate(1998, 3, 13), RemainingLeaveDays = 22, EmploymentDate = CreateUtcDate(2020, 2, 10), AnnualLeaveDays = 30
                    }
                };
                dbContext.Set<Employee>().AddRange(employees);
                dbContext.SaveChanges();
            }

            if (!dbContext.Customers.Any())
            {
                var customers = new[]
                {
                    new Customer {
                        Name = "John Doe", Email = "johndoe@example.com", PhoneNumber = "+1234567890", Country = "USA",
                        City = "New York", PostalCode = "100031", Street = "5th Avenue", Number = "101", Status = true,
                        BillingType = "Monthly", Tva = 10, Addition = NotApplicable, Date = CreateUtcDate(2019, 7, 10)
                    },
                    new Customer {
                        Name = "Jane Smith", Email = "janesmith@example.com", PhoneNumber = "+1987654321", Country = "Canada",
                        City = "Toronto", PostalCode = "105617", Street = "Bloor Street", Number = "45", Status = false,
                        BillingType = "Yearly", Tva = 7, Addition = NotApplicable, Date = CreateUtcDate(2023, 7, 11)
                    },
                    new Customer {
                        Name = "Michael Johnson", Email = "michaelj@example.com", PhoneNumber = "+1212345678", Country = "UK",
                        City = "London", PostalCode = "200543", Street = "Strans", Number = "3", Status = true,
                        BillingType = "Weekly", Tva = 5, Addition = NotApplicable, Date = CreateUtcDate(2012, 2, 10)
                    },
                    new Customer {
                        Name = "Emily Davis", Email = "emilyd@example.com", PhoneNumber = "+1345789123", Country = "Australia",
                        City = "Sydney", PostalCode = "231145", Street = "George Street", Number = "84", Status = true,
                        BillingType = "Monthly", Tva = 7, Addition = NotApplicable, Date = CreateUtcDate(2007, 2, 20)
                    },
                    new Customer {
                        Name = "Robert Brown", Email = "robertb@example.com", PhoneNumber = "+1478523690", Country = "Germany",
                        City = "Berlin", PostalCode = "632498", Street = "Unter den Linden", Number = "90", Status = false,
                        BillingType = "Weekly", Tva = 5, Addition = NotApplicable, Date = CreateUtcDate(2005, 7, 10)
                    }
                };
                dbContext.Customers.AddRange(customers);
                dbContext.SaveChanges();
            }

            return Ok("Data populated successfully.");
        }
    }
}
