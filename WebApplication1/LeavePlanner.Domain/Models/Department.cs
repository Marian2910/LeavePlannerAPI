using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Domain.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required!")]
        public required string Name { get; set; }
    }
}