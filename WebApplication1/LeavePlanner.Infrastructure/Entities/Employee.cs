using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Infrastructure.Entities
{
    public class Employee
    {
        [Key]
        public int Id { get; init; }

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

        public int JobId { get; init; }
        
        public int DepartmentId { get; init; }

        public Job? Job { get; init; }
        public Department? Department { get; init; }

        public DateTime Birthdate { get; init; }

        public DateTime EmploymentDate { get; init; } = DateTime.UtcNow;

        public int RemainingLeaveDays { get; set; } = 30;

        public int AnnualLeaveDays { get; set; } = 30;
    }
}