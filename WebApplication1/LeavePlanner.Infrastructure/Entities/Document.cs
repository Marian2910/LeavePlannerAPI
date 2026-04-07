using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Infrastructure.Entities
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Document name is required.")]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Type { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public byte[] File { get; set; } = Array.Empty<byte>();

        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;
    }
}
