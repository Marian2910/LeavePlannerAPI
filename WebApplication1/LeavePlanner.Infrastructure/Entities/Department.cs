using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Infrastructure.Entities
{
    public class Department
    {
        [Key]
        public int Id { get; init; }
        
        [Required(ErrorMessage = "Name is required!")]
        [MaxLength(50)]
        public required string Name { get; init; }
    }
}