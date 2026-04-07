using System.ComponentModel.DataAnnotations;

namespace Common.DTOs
{
    public abstract class EventDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The title is required.")]
        public required string Title { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "The start date is required.")]
        public required DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; } = null;

        public string? Location { get; set; } = string.Empty;
    }
}