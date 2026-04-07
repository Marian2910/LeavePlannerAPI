using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Infrastructure.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; init; }

        [Required(ErrorMessage = "Customer name is required!")]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Customer email is required!")]
        [EmailAddress(ErrorMessage = "Email not valid!")]
        [MaxLength(150)]
        public required string Email { get; init; }

        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; init; } = string.Empty;
        
        [MaxLength(100)]
        public string Country { get; init; } = string.Empty;
        
        [MaxLength(100)]
        public string City { get; init; } = string.Empty;

        [MaxLength(20)]
        public string PostalCode { get; init; } = string.Empty;

        [MaxLength(150)]
        public string Street { get; init; } = string.Empty;

        [MaxLength(20)]
        public string Number { get; init; } = string.Empty;

        public bool Status { get; set; } = true;

        [MaxLength(20)]
        public string BillingType { get; init; } = string.Empty;

        [Range(0, 100, ErrorMessage = "TVA must be between 0 and 100.")]
        public int Tva { get; init; }

        [MaxLength(250)]
        public string Addition { get; init; } = string.Empty;

        // Explicitly initializing the collection to avoid null reference warnings.
        public ICollection<Document> Documents { get; init; } = new HashSet<Document>();

        public DateTime Date { get; init; }
    }
}