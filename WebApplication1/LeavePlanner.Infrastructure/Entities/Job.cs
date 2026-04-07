using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Infrastructure.Entities
{
    public class Job
    {
        [Key]
        public int Id { get; init; }

        [Required(ErrorMessage = "Job title is required.")]
        [MaxLength(100)]
        public string Title { get; init; } = string.Empty;
    }
}