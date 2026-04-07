using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Domain.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; init; }

        [Required(ErrorMessage = "Customer name is required!")]
        public required string Name { get; init; }

        [Required(ErrorMessage = "Customer email is required!")]
        [EmailAddress(ErrorMessage = "Email not valid!")]
        public required string Email { get; init; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public required string PhoneNumber { get; init; }

        [Required(ErrorMessage = "The customer's country is required.")]
        public required string Country { get; init; }

        [Required(ErrorMessage = "The customer's city is required.")]
        public required string City { get; init; }

        [Required(ErrorMessage = "The postal code is required.")]
        public required string PostalCode { get; init; }

        [Required(ErrorMessage = "The street is required.")]
        public required string Street { get; init; }

        public string Number { get; init; } = string.Empty;

        public bool Status { get; init; } = true;

        [Required(ErrorMessage = "The billing type is required.")]
        public required string BillingType { get; init; }

        [Range(0, 100, ErrorMessage = "TVA must be between 0 and 100.")]
        public int Tva { get; init; }

        public string Addition { get; init; } = string.Empty;

        public ICollection<Document>? Documents { get; init; }

        [Required(ErrorMessage = "The date is required.")]
        public required DateTime Date { get; init; }
    }
}
