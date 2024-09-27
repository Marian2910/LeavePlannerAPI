using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Customer name is required!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Customer email is required!")]
        [EmailAddress(ErrorMessage = "Email not valid!")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public bool Status { get; set; }
        public string BillingType { get; set; }
        [Range(0, 100, ErrorMessage = "TVA must be between 0 and 100.")]
        public int Tva { get; set; }
        public string Addition { get; set; }
        public ICollection<Document>? Documents { get; set; }
        public DateTime Date {  get; set; }
    }
}
