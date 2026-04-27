using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Application.Interfaces;
using ArchitecturalWebsite.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace ArchitecturalWebsite.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaystackService _paystackService;
        private readonly IEmailService _emailService;
        private readonly ILogger<BookingService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;


        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IPaystackService paystackService, IEmailService emailService, ILogger<BookingService> logger, IConfiguration configuration, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paystackService = paystackService;
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
            _cache = cache;
        }

        public async Task<ApiResponse<BookingCreatedDto>> CreateBooking(CreateBookingDto dto)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(dto.ServiceId);
            if (service == null)
                return ApiResponse<BookingCreatedDto>.FailureResponse("Service not found");

            var booking = _mapper.Map<Booking>(dto);
            booking.ServiceName = service.Name;

            await _unitOfWork.Bookings.InsertAsync(booking);
            await _unitOfWork.SaveChangesAsync();
            InvalidateBookingCache();

            var whatsappNumber = _configuration["WhatsApp:BusinessNumber"] ?? "2349137838525";
            var message = $"Hi Neuma Lab! I'm interested in booking a consultation.\n\n" +
                         $"Name: {booking.ClientName}\n" +
                         $"Service: {service.Name}\n" +
                         $"Brief: {booking.Notes}\n\n" +
                         $"My Booking Reference: #{booking.Id}";
            var whatsappLink = $"https://wa.me/{whatsappNumber}?text={Uri.EscapeDataString(message)}";

            return ApiResponse<BookingCreatedDto>.SuccessResponse(new BookingCreatedDto
            {
                BookingId = booking.Id,
                ServiceName = service.Name,
                WhatsAppLink = whatsappLink,
                Message = "Booking created. Continue on WhatsApp."
            }, "Booking created successfully");
        }

        public async Task<ApiResponse<PriceSetDto>> SetAgreedPrice(int bookingId, SetAgreedPriceDto dto)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null)
                return ApiResponse<PriceSetDto>.FailureResponse("Booking not found");
            if (booking.Status != BookingStatus.Pending)
                return ApiResponse<PriceSetDto>.FailureResponse("Can only set price for pending bookings");

            if (!string.IsNullOrEmpty(booking.PaystackPaymentLink) && booking.PaymentStatus != PaymentStatus.FullyPaid)
                return ApiResponse<PriceSetDto>.SuccessResponse(new PriceSetDto
                {
                    BookingId = booking.Id,
                    AgreedPrice = booking.AgreedPrice!.Value,
                    PaymentLink = booking.PaystackPaymentLink,
                    PaymentTerms = booking.AllowHalfPayment
                        ? "You can choose to pay half now and the rest later."
                        : "Full payment required."
                }, "Payment link already exists for this booking");

            if (booking.PaymentStatus == PaymentStatus.FullyPaid)
                return ApiResponse<PriceSetDto>.FailureResponse("This booking has already been fully paid");


            var reference = $"NEUMA{booking.Id}{DateTime.Now:yyyyMMddHHmmss}";
            var paymentLink = await _paystackService.CreatePaymentLink(booking.ClientEmail, dto.AgreedPrice, reference, new { booking_id = bookingId });

            if (string.IsNullOrEmpty(paymentLink))
                return ApiResponse<PriceSetDto>.FailureResponse("Failed to generate payment link");

            booking.AgreedPrice = dto.AgreedPrice;
            booking.AllowHalfPayment = dto.AllowHalfPayment;
            booking.PaystackReference = reference;
            booking.PaystackPaymentLink = paymentLink;

            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<PriceSetDto>.SuccessResponse(new PriceSetDto
            {
                BookingId = booking.Id,
                AgreedPrice = booking.AgreedPrice.Value,
                PaymentLink = paymentLink,
                PaymentTerms = dto.AllowHalfPayment ? "You can choose to pay half now and the rest later." : "Full payment required."
            }, "Agreed price set successfully");
        }

        public async Task<ApiResponse<BookingResponseDto>> GetBookingById(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
                return ApiResponse<BookingResponseDto>.FailureResponse("Booking not found");

            var bookingDto = _mapper.Map<BookingResponseDto>(booking);
            return ApiResponse<BookingResponseDto>.SuccessResponse(bookingDto, "Booking retrieved successfully");
        }

        public async Task<ApiResponse<PaginatedResult<BookingResponseDto>>> GetAllBookings(BookingFilterParams filters)
        {
            var cacheKey = $"bookings_page{filters.PageNumber}_size{filters.PageSize}_status{filters.Status}_payment{filters.PaymentStatus}_service{filters.ServiceName}";

            if (_cache.TryGetValue(cacheKey, out PaginatedResult<BookingResponseDto> cachedResult))
            {
                _logger.LogInformation("Returning bookings from cache");
                return ApiResponse<PaginatedResult<BookingResponseDto>>.SuccessResponse(
                    cachedResult, "Bookings retrieved successfully");
            }

            var query = _unitOfWork.Bookings.GetQueryable();
            if (filters.Status.HasValue)
                query = query.Where(b => b.Status == filters.Status.Value);
            if (filters.PaymentStatus.HasValue)
                query = query.Where(b => b.PaymentStatus == filters.PaymentStatus.Value);
            if (!string.IsNullOrWhiteSpace(filters.ServiceName))
                query = query.Where(b => b.ServiceName.Contains(filters.ServiceName));

            var totalCount = await query.CountAsync();

            var pageNumber = filters.PageNumber < 1 ? 1 : filters.PageNumber;
            var pageSize = filters.PageSize < 1 ? 10 : filters.PageSize;

            var bookings = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            if (!bookings.Any())
                return ApiResponse<PaginatedResult<BookingResponseDto>>.SuccessResponse( new PaginatedResult<BookingResponseDto>
                    {
                        Data = new List<BookingResponseDto>(),
                        CurrentPage = pageNumber,
                        TotalPages = 0,
                        TotalCount = 0,
                        PageSize = pageSize
                    }, "No bookings found");

            var mappedData = _mapper.Map<IEnumerable<BookingResponseDto>>(bookings);

            var result = new PaginatedResult<BookingResponseDto>
            {
                Data = mappedData,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                TotalCount = totalCount,
                PageSize = pageSize
            };

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(2));
            _logger.LogInformation("Bookings cached with key: {CacheKey}", cacheKey);

            return ApiResponse<PaginatedResult<BookingResponseDto>>.SuccessResponse(result, "Bookings retrieved successfully");
        }

        public async Task<ApiResponse<BookingResponseDto>> UpdateBookingStatus(int id, UpdateBookingStatusDto dto)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
                return ApiResponse<BookingResponseDto>.FailureResponse("Booking not found");

            booking.Status = dto.Status;
            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            InvalidateBookingCache();

            var result = _mapper.Map<BookingResponseDto>(booking);
            return ApiResponse<BookingResponseDto>.SuccessResponse(result, "Booking status updated successfully");
        }

        public async Task<ApiResponse<bool>> CancelBooking(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
                return ApiResponse<bool>.FailureResponse("Booking not found");

            if (booking.Status == BookingStatus.Cancelled)
                return ApiResponse<bool>.FailureResponse("Booking is already cancelled");

            if (booking.PaymentStatus == PaymentStatus.FullyPaid)
                return ApiResponse<bool>.FailureResponse("Cannot cancel a fully paid booking. Please contact admin.");

            booking.Status = BookingStatus.Cancelled;
            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            InvalidateBookingCache();

            return ApiResponse<bool>.SuccessResponse(true, "Booking cancelled successfully");
        }

        private void InvalidateBookingCache()
        {
            if (_cache is MemoryCache memoryCache)
                memoryCache.Clear();
        }
    }
}

