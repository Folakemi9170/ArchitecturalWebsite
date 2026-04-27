using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ArchitecturalWebsite.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            var result = await _bookingService.CreateBooking(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("set-agreed-price/{bookingId}")]
        public async Task<IActionResult> SetAgreedPrice(int bookingId, [FromBody] SetAgreedPriceDto dto)
        {
            var result = await _bookingService.SetAgreedPrice(bookingId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var response = await _bookingService.GetBookingById(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings([FromQuery] BookingFilterParams filters)
        {
            var response = await _bookingService.GetAllBookings(filters);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusDto dto)
        {
            var response = await _bookingService.UpdateBookingStatus(id, dto);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var response = await _bookingService.CancelBooking(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
