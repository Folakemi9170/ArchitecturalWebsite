using ArchitecturalWebsite.Application.DTOs;

namespace ArchitecturalWebsite.Application.Interfaces
{
    public interface IBookingService
    {
        Task<ApiResponse<BookingCreatedDto>> CreateBooking(CreateBookingDto dto);
        Task<ApiResponse<PriceSetDto>> SetAgreedPrice(int bookingId, SetAgreedPriceDto dto);
        Task<ApiResponse<PaginatedResult<BookingResponseDto>>> GetAllBookings(BookingFilterParams filters);
        Task<ApiResponse<BookingResponseDto>> GetBookingById(int id);
        Task<ApiResponse<BookingResponseDto>> UpdateBookingStatus(int id, UpdateBookingStatusDto dto);
        Task<ApiResponse<bool>> CancelBooking(int id);
        //Task<ApiResponse<bool>> DeleteBooking(int id);
    }
}
