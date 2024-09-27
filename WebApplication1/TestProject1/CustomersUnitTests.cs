using AutoMapper;
using Common.DTOs;
using Domain.Helper;
using Domain.MappingProfiles;
using Domain.Services;
using Infrastructure.Configuration;
using Infrastructure.Entities;
using Infrastructure.Exceptions;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject1
{
    [TestFixture]
    public class CustomersUnitTests
    {
        private Domain.Models.Customer customer;
        private ApplicationDBContext dbContext;
        private CustomerRepository customerRepository;
        private ILogger<CustomerRepository> loggerRepository;
        private IMapper mapper;
        private ILogger<CustomerService> loggerService;
        private CustomerService customerService;

        [SetUp]
        public void Setup()
        {
            customer = CreateTestCustomer("Ana Maria", "ana.maria@example.com", "0745825640", "Romania", "Cluj-Napoca", "400672", "Mehedinti", "50", "Monthly", 20, "Info", DateTime.Now);

            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            dbContext = new ApplicationDBContext(options);

            loggerRepository = new Mock<ILogger<CustomerRepository>>().Object;
            loggerService = new Mock<ILogger<CustomerService>>().Object;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfiles>();
            });
            mapper = config.CreateMapper();

            customerRepository = new CustomerRepository(dbContext, loggerRepository);
            customerService = new CustomerService(customerRepository, mapper, loggerService);

            dbContext.Customers.Add(mapper.Map<Infrastructure.Entities.Customer>(customer));
            dbContext.SaveChanges();
            dbContext.ChangeTracker.Clear();
        }

        [TearDown]
        public void Cleanup()
        {
            dbContext.Dispose();
        }

        private Domain.Models.Customer CreateTestCustomer(string name, string email,
            string phoneNumber, string country, string city, string postalCode, string street, string number, string billingType, int tva,
            string addition, DateTime date)
        {
            return new Domain.Models.Customer
            {
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber,
                Country = country,
                City = city,
                PostalCode = postalCode,
                Street = street,
                Number = number,
                BillingType = billingType,
                Tva = tva,
                Addition = addition,
                Date = date
            };
        }

        private async Task<Infrastructure.Entities.Customer> GetCustomerByEmailAsync(string email)
        {
            return await dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        private async Task<Infrastructure.Entities.Customer> GetCustomerByNameAsync(string name)
        {
            return await dbContext.Customers.FirstOrDefaultAsync(c => c.Name == name);
        }

        [Test]
        public void Customer_ShouldPassValidation_WhenModelIsValid()
        {
            var validationResult = ValidationHelper.ValidateModel(customer);
            Assert.That(validationResult.Any(), Is.False);
        }

        [Test]
        [TestCase("abc")]
        [TestCase("1")]
        public void Customer_ShouldFailValidation_WhenEmailIsInvalid(string email)
        {
            customer.Email = email;

            var validationResult = ValidationHelper.ValidateModel(customer);

            Assert.That(validationResult.Any(), Is.True);
            Assert.That(validationResult.Any(r => r.ErrorMessage == "Email not valid!"), Is.True);
        }

        [Test]
        [TestCase(150)]
        [TestCase(200)]
        public void Customer_ShouldFailValidation_WhenTVAIsInvalid(int tva)
        {
            customer.Tva = tva;

            var validationResult = ValidationHelper.ValidateModel(customer);

            Assert.That(validationResult.Any(), Is.True);
            Assert.That(validationResult.Any(r => r.ErrorMessage == "TVA must be between 0 and 100."), Is.True);
        }

        [Test]
        [TestCase("")]
        public void Customer_ShouldFailValidation_WhenNameIsInvalid(string name)
        {
            customer.Name = name;

            var validationResult = ValidationHelper.ValidateModel(customer);

            Assert.That(validationResult.Any(), Is.True);
            Assert.That(validationResult.Any(r => r.ErrorMessage == "Customer name is required!"), Is.True);
        }

        [Test]
        public async Task AddCustomerAsync_ShouldAddCustomer()
        {
            var newCustomer = CreateTestCustomer("Ion Pop", "ion.pop@example.com", "0745825640", "Romania", "Bucharest", "400672", "Victoriei", "30", "Yearly", 15, "Info", DateTime.Now);
            var customerEntity = mapper.Map<Infrastructure.Entities.Customer>(newCustomer);

            await customerRepository.AddCustomerAsync(customerEntity);

            var addedCustomer = await GetCustomerByEmailAsync(customerEntity.Email);
            Assert.That(addedCustomer, Is.Not.Null);
            Assert.That(addedCustomer.Name, Is.EqualTo("Ion Pop"));
        }

        [Test]
        [TestCase("Ana Maria")]
        public async Task DeleteCustomerAsync_ShouldMarkCustomerAsInactive(string name)
        {
            var customerToDelete = await GetCustomerByNameAsync(name);
            Assert.That(customerToDelete, Is.Not.Null);

            await customerRepository.DeleteCustomerAsync(customerToDelete.Id);
            var deletedCustomer = await dbContext.Customers.FindAsync(customerToDelete.Id);

            Assert.That(deletedCustomer.Status, Is.False);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllCustomers()
        {
            var customers = await customerRepository.GetAllAsync();

            Assert.That(customers, Is.Not.Null);
            Assert.That(customers.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllPagedAsync_ShouldReturnPagedCustomers()
        {
            var customers = await customerService.GetCustomersAsync(1, 10);

            Assert.That(customers, Is.Not.Null);
            Assert.That(customers.Items.Count(), Is.EqualTo(1));
        }

        [Test]
        [TestCase("Ana")]
        public async Task SearchCustomersAsync_ShouldReturnMatchingCustomers(string name)
        {
            var customers = await customerRepository.SearchCustomersAsync(name);

            Assert.That(customers, Is.Not.Null);
            Assert.That(customers.Any(c => c.Name.Contains(name)), Is.True);
        }

        [Test]
        public async Task GetTotalCustomerCountAsync_ShouldReturnCorrectCount()
        {
            var totalCount = await customerRepository.GetTotalCustomerCountAsync();

            Assert.That(totalCount, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateCustomerAsync_ShouldUpdateCustomer()
        {
            var customerToUpdate = await GetCustomerByNameAsync("Ana Maria");
            Assert.That(customerToUpdate, Is.Not.Null);

            customerToUpdate.City = "Bucharest";

            await customerRepository.UpdateCustomerAsync(customerToUpdate);

            var updatedCustomer = await dbContext.Customers.FindAsync(customerToUpdate.Id);

            Assert.That(updatedCustomer, Is.Not.Null);
            Assert.That(updatedCustomer.City, Is.EqualTo("Bucharest"));
        }

        [Test]
        public async Task UpdateCustomer_ShouldUpdateCustomer()
        {
            var customerToUpdate = await customerService.GetCustomerByIdAsync(1);

            customerToUpdate.City = "Bucharest";

            var updatedCustomer = mapper.Map<Domain.Models.Customer>(customerToUpdate);
            await customerService.UpdateCustomer(updatedCustomer);
        }

        [Test]
        public async Task AddCustomer_ShouldAddCustomer()
        {
            var newCustomer = new Domain.Models.Customer
            {
                Name = "Laura Matei",
                Email = "laura.matei@example.com",
                PhoneNumber = "0795625640",
                Country = "Romania",
                City = "Bucharest",
                PostalCode = "400672",
                Street = "Victoriei",
                Number = "30",
                BillingType = "Yearly",
                Tva = 15,
                Addition = "Info",
                Date = DateTime.Now
            };

            await customerService.AddCustomer(newCustomer);

            var addedCustomer = await GetCustomerByEmailAsync("laura.matei@example.com");

            Assert.That(addedCustomer, Is.Not.Null);
            Assert.That(addedCustomer.Name, Is.EqualTo("Laura Matei"));
        }

        [Test]
        [TestCase("Diana Matei", "diana.matei@example.com", "0795625640", "Romania", "Bucharest", "400672", "Victoriei", "30", "Yearly", 15, "Info")]
        public async Task DeleteCustomer_ShouldDeleteCustomer(string name, string email, string phoneNumber, string country, string city, string postalCode, string street, string number, string billingType, int tva, string addition)
        {
            var newCustomer = new Domain.Models.Customer
            {
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber,
                Country = country,
                City = city,
                PostalCode = postalCode,
                Street = street,
                Number = number,
                BillingType = billingType,
                Tva = tva,
                Addition = addition,
                Date = DateTime.Now
            };

            await customerService.AddCustomer(newCustomer);

            var addedCustomer = await GetCustomerByEmailAsync(email);
            Assert.That(addedCustomer, Is.Not.Null);
            Assert.That(addedCustomer.Name, Is.EqualTo(name));

            await customerService.DeleteCustomer(addedCustomer.Id);
            var totalCount = dbContext.Customers.Count();
            Assert.That(totalCount, Is.EqualTo(2));

            var deletedCustomer = await dbContext.Customers.FindAsync(addedCustomer.Id);
            Assert.That(deletedCustomer.Status, Is.False);
        }

        [Test]
        public async Task GetCustomerByIdAsync_ShouldReturnCustomer()
        {
            var customer = await customerService.GetCustomerByIdAsync(1);

            Assert.That(customer, Is.Not.Null);
            Assert.That(customer.City, Is.EqualTo("Cluj-Napoca"));
        }

        [Test]
        public async Task SearchCustomersByNameAsync_ShouldReturnCustomers()
        {
            var pagedResultDto = await customerService.SearchCustomersByNameAsync("Ana", 1, 10);
            var customers = pagedResultDto.Items;

            Assert.That(customers, Is.Not.Null);
            Assert.That(customers.Count(), Is.EqualTo(1));
            Assert.That(customers.First().Tva, Is.EqualTo(20));

            Assert.ThrowsAsync<NullEntity>(async () => await customerService.SearchCustomersByNameAsync("Ion", 1, 10));
        }
    }
}
