namespace ArchitecturalWebsite.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
        Task SendPaymentConfirmationEmailAsync(string toEmail, string clientName, string serviceName, decimal amountPaid);
        Task SendIncompletePaymentEmailAsync(int bookingId, string clientName, string serviceName, decimal agreedPrice, decimal amountPaid, decimal shortfall);
    }
}
