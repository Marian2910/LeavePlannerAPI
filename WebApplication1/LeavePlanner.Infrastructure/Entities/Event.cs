using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Infrastructure.Entities
{
    public class Event
    {
        [Key]
        public int Id { get; init; }

        [Required(ErrorMessage = "Event title is required.")]
        [MaxLength(150)]
        public string Title { get; init; } = string.Empty;

        public DateTime StartDate { get; init; }

        public DateTime? EndDate { get; init; }
    }
}