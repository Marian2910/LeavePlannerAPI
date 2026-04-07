using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Domain.Models
{
    public class Job
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required!")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Role is required!")]
        public required string Role { get; set; }
    }
}