using System.ComponentModel.DataAnnotations;

namespace Common.DTOs
{
    public class PersonalEventDto
    {
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int EmployeeId { get; set; }
    }
}
