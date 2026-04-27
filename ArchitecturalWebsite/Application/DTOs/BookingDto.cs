using ArchitecturalWebsite.Domain;

namespace ArchitecturalWebsite.Application.DTOs
{
    public class CreateBookingDto
    {
        public required string ClientName { get; set; }
        public required string ClientEmail { get; set; }
        public required string ClientPhone { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public int ServiceId { get; set; }
    }
    public class BookingCreatedDto
    {
        public int BookingId { get; set; }
        public string WhatsAppLink { get; set; }
        public string Message { get; set; }
        public BookingStatus Status { get; set; }
        public string ServiceName { get; set; }
    }

    public class SetAgreedPriceDto
    {
        public decimal AgreedPrice { get; set; }
        public bool AllowHalfPayment { get; set; } = false;
    }

    public class PriceSetDto
    {
        public int BookingId { get; set; }
        public decimal AgreedPrice { get; set; }
        public string PaymentLink { get; set; }
        public string PaymentTerms { get; set; }
    }

    //Get all bookings
    public class BookingResponseDto
        {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }
        public string Address { get; set; }
        public string? Notes { get; set; }
        public string ServiceName { get; set; }
        public BookingStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal? AgreedPrice { get; set; }
        public decimal? AmountPaid { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PaystackPaymentLink { get; set; }
    }

    public class UpdateBookingStatusDto
    {
        public BookingStatus Status { get; set; }
    }

    // Payload models
    public class PaystackWebhookPayload
    {
        public string Event { get; set; } = string.Empty;
        public PaystackWebhookData? Data { get; set; }
    }

    public class PaystackWebhookData
    {
        public string Reference { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
