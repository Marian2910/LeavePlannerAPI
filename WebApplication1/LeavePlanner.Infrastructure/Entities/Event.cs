using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Infrastructure.Entities
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Event title is required.")]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }
    }
}