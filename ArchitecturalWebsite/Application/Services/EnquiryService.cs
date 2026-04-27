using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Application.Interfaces;
using ArchitecturalWebsite.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ArchitecturalWebsite.Application.Services
{
    public class EnquiryService : IEnquiryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EnquiryService> _logger;
        private readonly IMemoryCache _cache;

        public EnquiryService(IUnitOfWork unitOfWork, IMapper mapper, 
            ILogger<EnquiryService> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<ApiResponse<bool>> CreateEnquiry(CreateEnquiryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                return ApiResponse<bool>.FailureResponse("Full name is required");
            if (string.IsNullOrWhiteSpace(dto.Email))
                return ApiResponse<bool>.FailureResponse("Email is required");
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                return ApiResponse<bool>.FailureResponse("Phone number is required");
            if (string.IsNullOrWhiteSpace(dto.Subject))
                return ApiResponse<bool>.FailureResponse("Subject is required");
            if (string.IsNullOrWhiteSpace(dto.Message))
                return ApiResponse<bool>.FailureResponse("Message is required");

            var enquiry = _mapper.Map<Enquiry>(dto);
            await _unitOfWork.Enquiries.InsertAsync(enquiry);
            await _unitOfWork.SaveChangesAsync();
            InvalidateEnquiryCache();

            _logger.LogInformation("New enquiry received from {Email}", dto.Email);
            return ApiResponse<bool>.SuccessResponse(true, "Enquiry submitted successfully");
        }

        public async Task<ApiResponse<PaginatedResult<EnquiryResponseDto>>> GetAllEnquiries(EnquiryFilterParams filters)
        {
            var cacheKey = $"enquiries_page{filters.PageNumber}_size{filters.PageSize}_isread{filters.IsRead}";

            if (_cache.TryGetValue(cacheKey, out PaginatedResult<EnquiryResponseDto> cachedResult))
            {
                _logger.LogInformation("Returning enquiries from cache");
                return ApiResponse<PaginatedResult<EnquiryResponseDto>>.SuccessResponse(
                    cachedResult, "Enquiries retrieved successfully");
            }

            var query = _unitOfWork.Enquiries.GetQueryable();

            if (filters.IsRead.HasValue)
                query = query.Where(e => e.IsRead == filters.IsRead.Value);

            var totalCount = await query.CountAsync();

            var pageNumber = filters.PageNumber < 1 ? 1 : filters.PageNumber;
            var pageSize = filters.PageSize < 1 ? 10 : filters.PageSize;

            var enquiries = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!enquiries.Any())
                return ApiResponse<PaginatedResult<EnquiryResponseDto>>.SuccessResponse(
                    new PaginatedResult<EnquiryResponseDto>
                    {
                        Data = new List<EnquiryResponseDto>(),
                        CurrentPage = pageNumber,
                        TotalPages = 0,
                        TotalCount = 0,
                        PageSize = pageSize
                    }, "No enquiries found");

            var mappedData = _mapper.Map<IEnumerable<EnquiryResponseDto>>(enquiries);

            var result = new PaginatedResult<EnquiryResponseDto>
            {
                Data = mappedData,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                TotalCount = totalCount,
                PageSize = pageSize
            };

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(2));
            return ApiResponse<PaginatedResult<EnquiryResponseDto>>.SuccessResponse(
                result, "Enquiries retrieved successfully");
        }

        public async Task<ApiResponse<EnquiryResponseDto>> GetEnquiryById(int id)
        {
            var enquiry = await _unitOfWork.Enquiries.GetByIdAsync(id);
            if (enquiry == null)
                return ApiResponse<EnquiryResponseDto>.FailureResponse("Enquiry not found");

            var result = _mapper.Map<EnquiryResponseDto>(enquiry);
            return ApiResponse<EnquiryResponseDto>.SuccessResponse(result, "Enquiry retrieved successfully");
        }

        public async Task<ApiResponse<bool>> MarkAsRead(int id)
        {
            var enquiry = await _unitOfWork.Enquiries.GetByIdAsync(id);
            if (enquiry == null)
                return ApiResponse<bool>.FailureResponse("Enquiry not found");

            if (enquiry.IsRead)
                return ApiResponse<bool>.SuccessResponse(true, "Enquiry is already marked as read");

            enquiry.IsRead = true;
            _unitOfWork.Enquiries.Update(enquiry);
            await _unitOfWork.SaveChangesAsync();
            InvalidateEnquiryCache();

            return ApiResponse<bool>.SuccessResponse(true, "Enquiry marked as read successfully");
        }

        private void InvalidateEnquiryCache()
        {
            if (_cache is MemoryCache memoryCache)
                memoryCache.Clear();
        }
    }
}
