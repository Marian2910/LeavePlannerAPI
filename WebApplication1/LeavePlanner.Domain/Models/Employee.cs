using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Domain.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "FirstName is required!")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required!")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Email not valid!")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "The employee's job is required.")]
        public required Job Job { get; set; }

        [Required(ErrorMessage = "The employee's department is required.")]
        public required Department Department { get; set; }

        public ICollection<PersonalEvent> PersonalEvents { get; set; } = new HashSet<PersonalEvent>();

        [Required(ErrorMessage = "Birthdate is required.")]
        public required DateTime Birthdate { get; set; }

        public DateTime EmploymentDate { get; set; } = DateTime.UtcNow;

        public int RemainingLeaveDays { get; set; } = 0;

        public int AnnualLeaveDays { get; set; } = 30;
    }
}
