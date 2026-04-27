using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Application.Interfaces;
using ArchitecturalWebsite.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ArchitecturalWebsite.Presentation.Controllers
{
    [Route("api/webhooks")]
    [ApiController]
    public class PaystackWebhookController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public PaystackWebhookController(IUnitOfWork unitOfWork, IEmailService emailService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpPost("paystack")]
        public async Task<IActionResult> Handle()
        {
            var paystackSecret = _configuration["Paystack:SecretKey"]!; // confirm that is genuinely from paystack
            Request.Headers.TryGetValue("x-paystack-signature", out var signature);

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var hash = ComputeHmacSha512(body, paystackSecret);
            if (!hash.Equals(signature.ToString(), StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var payload = JsonSerializer.Deserialize<PaystackWebhookPayload>(body, //Parse the event

                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload?.Event == "charge.success")
            {
                var reference = payload.Data?.Reference;
                var booking = await _unitOfWork.Bookings.GetByConditionAsync(b => b.PaystackReference == reference); ;

                if (booking != null && booking.PaymentStatus != PaymentStatus.FullyPaid)
                {
                    var amountPaid = payload.Data!.Amount / 100m; // convert from kobo

                    booking.AmountPaid = (booking.AmountPaid ?? 0) + amountPaid;

                    if (booking.AmountPaid < booking.AgreedPrice)
                    {
                        // Incomplete payment, save the partial payment and notify admin
                        booking.PaymentStatus = PaymentStatus.HalfPaid;
                        _unitOfWork.Bookings.Update(booking);
                        await _unitOfWork.SaveChangesAsync();

                        var shortfall = booking.AgreedPrice - booking.AmountPaid;
                        await _emailService.SendIncompletePaymentEmailAsync(
                            booking.Id,
                            booking.ClientName,
                            booking.ServiceName,
                            booking.AgreedPrice!.Value,
                            booking.AmountPaid!.Value,
                            shortfall!.Value
                        );

                        return Ok();
                    }

                    // Full payment received (either first time or balance just completed it)
                    booking.PaymentStatus = PaymentStatus.FullyPaid;
                    booking.Status = BookingStatus.Confirmed;

                    _unitOfWork.Bookings.Update(booking);
                    await _unitOfWork.SaveChangesAsync();

                    await _emailService.SendPaymentConfirmationEmailAsync(
                        booking.ClientEmail,
                        booking.ClientName,
                        booking.ServiceName,
                        booking.AmountPaid!.Value
                    );
                }
            }

            return Ok();
        }

        private static string ComputeHmacSha512(string data, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using var hmac = new HMACSHA512(keyBytes);
            var hash = hmac.ComputeHash(dataBytes);
            return Convert.ToHexString(hash).ToLower();
        }

    }
}
