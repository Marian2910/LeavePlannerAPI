using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Domain.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        public required string Type { get; set; }

        public required DateTime Date { get; set; }

        [Required(ErrorMessage = "File content is required")]
        public required byte[] File { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        public required Customer Customer { get; set; }
    }
}
