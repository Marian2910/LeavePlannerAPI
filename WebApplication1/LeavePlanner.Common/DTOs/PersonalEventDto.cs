using System.ComponentModel.DataAnnotations;

namespace Common.DTOs
{
    public class PersonalEventDto
    {
        [Required(ErrorMessage = "The title is required.")]
        public required string Title { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "The start date is required.")]
        public required DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; } = null;

        [Required(ErrorMessage = "The employee ID is required.")]
        public int EmployeeId { get; set; }
    }
}