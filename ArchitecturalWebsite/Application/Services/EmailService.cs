using ArchitecturalWebsite.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace ArchitecturalWebsite.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        
        public EmailService(IConfiguration configuration)
            {
                _configuration = configuration;
            }
        
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration["Email:FromEmail"]));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = message };

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync( _configuration["Email:Server"], int.Parse(_configuration["Email:Port"]), SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                _configuration["Email:Username"],
                _configuration["Email:Password"]
                );

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
        }

        public async Task SendPaymentConfirmationEmailAsync(string toEmail, string clientName, string serviceName, decimal amountPaid)
        {
            var subject = "Payment Confirmed";

            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <p>Dear {clientName},</p>
                    <p>Your payment of <strong>{amountPaid:C}</strong> has been confirmed to <strong>Neuma Lab</strong>.</p>
                    <p>This automatically validates the service, <strong>{serviceName}</strong>, you booked.</p>

                    <p>If you have any questions, please reach out via WhatsApp.</p>
                    <p>Best regards,<br><strong>Neuma Lab Team</strong></p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendIncompletePaymentEmailAsync( int bookingId, string clientName, string serviceName, decimal agreedPrice, decimal amountPaid, decimal shortfall)
        {
            var adminEmail = _configuration["Email:FromEmail"];
            var subject = $"⚠️ Incomplete Payment — Booking #{bookingId}";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h3>Incomplete Payment Alert</h3>
                    <p><strong>Booking ID:</strong> #{bookingId}</p>
                    <p><strong>Client:</strong> {clientName}</p>
                    <p><strong>Service:</strong> {serviceName}</p>
                    <p><strong>Agreed Price:</strong> ₦{agreedPrice:N2}</p>
                    <p><strong>Amount Paid So Far:</strong> ₦{amountPaid:N2}</p>
                    <p><strong>Balance Remaining:</strong> ₦{shortfall:N2}</p>
                    <p>The client has been partially charged. Awaiting balance payment.</p>
                </body>
                </html>";

            await SendEmailAsync(adminEmail!, subject, body);
        }
    }
}
