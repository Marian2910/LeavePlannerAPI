using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Infrastructure.Entities
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        public int JobId { get; set; }
        
        public int DepartmentId { get; set; }

        public Job? Job { get; set; }
        public Department? Department { get; set; }

        public ICollection<PersonalEvent> PersonalEvents { get; set; } = new List<PersonalEvent>();

        public DateTime Birthdate { get; set; }

        public DateTime EmploymentDate { get; set; } = DateTime.UtcNow;

        public int RemainingLeaveDays { get; set; } = 30;

        public int AnnualLeaveDays { get; set; } = 30;
    }
}