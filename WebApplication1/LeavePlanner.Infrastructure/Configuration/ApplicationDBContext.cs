using LeavePlanner.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeavePlanner.Infrastructure.Configuration
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<PersonalEvent> PersonalEvents { get; set; }
    }
}