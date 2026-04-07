namespace LeavePlanner.Infrastructure.Entities
{
    public class PersonalEvent : Event
    {
        // Foreign key
        public int EmployeeId { get; set; }

        // Navigation property
        public Employee Employee { get; set; } = null!;
    }
}
