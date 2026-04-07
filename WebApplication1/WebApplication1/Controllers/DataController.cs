using LeavePlanner.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using LeavePlanner.Infrastructure.Entities;

namespace LeavePlanner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController(ApplicationDbContext dbContext) : Controller
    {
        
        [HttpPost("createData")]
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
                    new Department { Name = "Finance" },
                    new Department { Name = "IT/Tech" }
                };
                dbContext.Departments.AddRange(departments);
                dbContext.SaveChanges();
            }

            // Retrieve jobs and departments only once
            var jobsByTitle = dbContext.Jobs.ToDictionary(j => j.Title);
            var departmentsByName = dbContext.Departments.ToDictionary(d => d.Name);

            if (!dbContext.Employees.Any())
            {
                var employees = new[]
                {
                    new Employee {
                        Job = jobsByTitle["Finance Manager"], Department = departmentsByName["Finance"],
                        Birthdate = new DateTime(1995, 10, 22), RemainingLeaveDays = 30, EmploymentDate = new DateTime(2019, 05, 01), AnnualLeaveDays = 31
                    },
                    new Employee {
                        Job = jobsByTitle["Front-End Developer"], Department = departmentsByName["IT/Tech"],
                        Birthdate = new DateTime(1995, 10, 22), RemainingLeaveDays = 28, EmploymentDate = new DateTime(2021, 10, 15), AnnualLeaveDays = 29
                    },
                    new Employee {
                        Job = jobsByTitle["Back-End Developer"], Department = departmentsByName["IT/Tech"],
                        Birthdate = new DateTime(1995, 10, 22), RemainingLeaveDays = 25, EmploymentDate = new DateTime(2023, 10, 01), AnnualLeaveDays = 27
                    },
                    new Employee {
                        Job = jobsByTitle["Full-Stack Developer"], Department = departmentsByName["IT/Tech"],
                        Birthdate = new DateTime(1998, 03, 13), RemainingLeaveDays = 22, EmploymentDate = new DateTime(2020, 02, 10), AnnualLeaveDays = 30
                    }
                };
                dbContext.Employees.AddRange(employees);
                dbContext.SaveChanges();
            }

            if (!dbContext.Customers.Any())
            {
                var customers = new[]
                {
                    new Customer {
                        Name = "John Doe", Email = "johndoe@example.com", PhoneNumber = "+1234567890", Country = "USA",
                        City = "New York", PostalCode = "100031", Street = "5th Avenue", Number = "101", Status = true,
                        BillingType = "Monthly", Tva = 10, Addition = "N/A", Date = new DateTime(2019, 07, 10)
                    },
                    new Customer {
                        Name = "Jane Smith", Email = "janesmith@example.com", PhoneNumber = "+1987654321", Country = "Canada",
                        City = "Toronto", PostalCode = "105617", Street = "Bloor Street", Number = "45", Status = false,
                        BillingType = "Yearly", Tva = 7, Addition = "N/A", Date = new DateTime(2023, 07, 11)
                    },
                    new Customer {
                        Name = "Michael Johnson", Email = "michaelj@example.com", PhoneNumber = "+1212345678", Country = "UK",
                        City = "London", PostalCode = "200543", Street = "Strans", Number = "3", Status = true,
                        BillingType = "Weekly", Tva = 5, Addition = "N/A", Date = new DateTime(2012, 02, 10)
                    },
                    new Customer {
                        Name = "Emily Davis", Email = "emilyd@example.com", PhoneNumber = "+1345789123", Country = "Australia",
                        City = "Sydney", PostalCode = "231145", Street = "George Street", Number = "84", Status = true,
                        BillingType = "Monthly", Tva = 7, Addition = "N/A", Date = new DateTime(2007, 02, 20)
                    },
                    new Customer {
                        Name = "Robert Brown", Email = "robertb@example.com", PhoneNumber = "+1478523690", Country = "Germany",
                        City = "Berlin", PostalCode = "632498", Street = "Unter den Linden", Number = "90", Status = false,
                        BillingType = "Weekly", Tva = 5, Addition = "N/A", Date = new DateTime(2005, 07, 10)
                    }
                };
                dbContext.Customers.AddRange(customers);
                dbContext.SaveChanges();
            }

            return Ok("Data populated successfully.");
        }
    }
}