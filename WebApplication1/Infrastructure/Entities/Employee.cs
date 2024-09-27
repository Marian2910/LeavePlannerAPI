using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "FirstName is required!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required!")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Email not valid!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }
        public Job Job { get; set; }
        public Department Department { get; set; }
        public ICollection<PersonalEvent> PersonalEvents { get; set; }
        public DateTime Birthdate { get; set; }
        public DateTime EmploymentDate { get; set; } = DateTime.Now;
        public int RemainingLeaveDays { get; set; } = 30;
        public int AnnualLeaveDays { get; set; } = 30;
    }
}