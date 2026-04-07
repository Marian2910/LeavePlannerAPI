using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Infrastructure.Entities
{
    public class Job
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Job title is required.")]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Role { get; set; }
    }
}