using Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Entities;

namespace ProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        public DataController(ApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        [HttpPost("createData")]
        public IActionResult CreateData()
        {
            using (var dbContext = _dbContext)
            {
                if (!dbContext.Jobs.Any())
                {
                    var job1 = new Job { Title = "Finance Manager", Role = "admin" };
                    var job2 = new Job { Title = "Front-End Developer", Role = "user" };
                    var job3 = new Job { Title = "Back-End Developer", Role = "user" };
                    var job4 = new Job { Title = "Full-Stack Developer", Role = "user" };

                    _dbContext.Jobs.AddRange(job1, job2, job3, job4);
                    _dbContext.SaveChanges();
                }

                if (!dbContext.Departments.Any())
                {
                    var departament1 = new Department { Name = "Finance" };
                    var departament2 = new Department { Name = "IT/Tech" };

                    _dbContext.Departments.AddRange(departament1, departament2);
                    _dbContext.SaveChanges();
                }

                var financeManager = _dbContext.Jobs.FirstOrDefault(j => j.Title == "Finance Manager");
                var frontendDeveloper = _dbContext.Jobs.FirstOrDefault(j => j.Title == "Front-End Developer");
                var backendDeveloper = _dbContext.Jobs.FirstOrDefault(j => j.Title == "Back-End Developer");
                var fullStackDeveloper = _dbContext.Jobs.FirstOrDefault(j => j.Title == "Full-Stack Developer");
                var financeDepartament = _dbContext.Departments.FirstOrDefault(d => d.Name == "Finance");
                var ITDepartament = _dbContext.Departments.FirstOrDefault(d => d.Name == "IT/Tech");


                if (!dbContext.Employees.Any())
                {
                    var employee1 = new Employee { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@red-to-blue.com", Password = "janedoe", Job = financeManager, Department = financeDepartament, Birthdate = new DateTime(1995, 10, 22), RemainingLeaveDays = 30, EmploymentDate = new DateTime(2019, 05, 01), AnnualLeaveDays = 31 };
                    var employee2 = new Employee { FirstName = "Ted", LastName = "Marshal", Email = "ted.marshal@red-to-blue.com", Password = "tedmarshal", Job = frontendDeveloper, Department = ITDepartament, Birthdate = new DateTime(1995, 10, 22), RemainingLeaveDays = 28, EmploymentDate = new DateTime(2021, 10, 15) ,AnnualLeaveDays = 29};
                    var employee3 = new Employee { FirstName = "Paula", LastName = "Smith", Email = "paula.smith@red-to-blue.com", Password = "paulasmith", Job = backendDeveloper, Department = ITDepartament, Birthdate = new DateTime(1995, 10, 22), RemainingLeaveDays = 25, EmploymentDate = new DateTime(2023, 10, 01) , AnnualLeaveDays = 27};
                    var employee4 = new Employee { FirstName = "Millie", LastName = "James", Email = "millie.james@red-to-blue.com", Password = "milliejames", Job = fullStackDeveloper, Department = ITDepartament, Birthdate = new DateTime(1998, 03, 13), RemainingLeaveDays = 22, EmploymentDate = new DateTime(2020, 02, 10) , AnnualLeaveDays = 30};

                    _dbContext.Employees.AddRange(employee1, employee2, employee3, employee4);
                    _dbContext.SaveChanges();
                }

                if (!dbContext.Customers.Any())
                {
                    var customer1 = new Customer { Name = "John Doe", Email = "johndoe@example.com", PhoneNumber = "+1234567890", Country = "USA", City = "New York", PostalCode = "100031", Street = "5th Avenue", Number = "101", Status = true, BillingType = "Monthly", Tva = 10, Addition = "N/A", Date=new DateTime(2019, 07, 10) };
                    var customer2 = new Customer { Name = "Jane Smith", Email = "janesmith@example.com", PhoneNumber = "+1987654321", Country = "Canada", City = "Toronto", PostalCode = "105617", Street = "Bloor Street", Number = "45", Status = false, BillingType = "Yearly", Tva = 7, Addition = "N/A", Date = new DateTime(2023, 07, 11) };
                    var customer3 = new Customer { Name = "Michael Johnson", Email = "michaelj@example.com", PhoneNumber = "+1212345678", Country = "UK", City = "London", PostalCode = "200543", Street = "Strans", Number = "3", Status = true, BillingType = "Weekly", Tva = 5, Addition = "N/A", Date = new DateTime(2012, 02, 10) };
                    var customer4 = new Customer { Name = "Emily Davis", Email = "emilyd@example.com", PhoneNumber = "+1345789123", Country = "Australia", City = "Sydney", PostalCode = "231145", Street = "George Street", Number = "84", Status = true, BillingType = "Monthly", Tva = 7, Addition = "N/A", Date = new DateTime(2007, 02, 20) };
                    var customer5 = new Customer { Name = "Robert Brown", Email = "robertb@example.com", PhoneNumber = "+1478523690", Country = "Germany", City = "Berlin", PostalCode = "632498", Street = "Unter den Linden", Number = "90", Status = false, BillingType = "Weekly", Tva = 5, Addition = "N/A", Date = new DateTime(2005, 07, 10) };

                    _dbContext.Customers.AddRange(customer1, customer2, customer3, customer4, customer5);
                    _dbContext.SaveChanges();
                }
                return Ok();
            }
        }
    }
}
