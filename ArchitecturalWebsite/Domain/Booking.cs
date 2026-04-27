namespace ArchitecturalWebsite.Domain
{
    public class Booking : BaseEntity
    {
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }
        public string? Address { get; set; } 
        public string? Notes { get; set; } //additional information from client
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public Service Service { get; set; }

        public decimal? AgreedPrice { get; set; }
        public bool AllowHalfPayment { get; set; } = false;

        public decimal? AmountPaid { get; set; }
        public string? PaystackReference { get; set; }
        public string? PaystackPaymentLink { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    }

    public enum PaymentStatus
    {
        Pending,
        HalfPaid,
        FullyPaid,
        Refunded,
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }
}
