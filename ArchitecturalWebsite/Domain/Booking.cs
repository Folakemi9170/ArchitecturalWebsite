namespace ArchitecturalWebsite.Domain
{
    public class Booking : BaseEntity
    {
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public BookingStatus Status { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }
}
