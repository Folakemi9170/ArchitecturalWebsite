using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Application.Interfaces;
using ArchitecturalWebsite.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static ArchitecturalWebsite.Application.DTOs.ReviewDto;

namespace ArchitecturalWebsite.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;
        private readonly IMemoryCache _cache;
        private const string ApprovedCacheKey = "approved_reviews";
        private const string AllReviewsCacheKey = "all_reviews";

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReviewService> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<ApiResponse<ReviewResponseDto>> CreateReview(CreateReviewDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.ClientName))
                errors.Add("Client name is required");
            if (string.IsNullOrWhiteSpace(dto.Review))
                errors.Add("Review is required");
            if (dto.Rating < 1 || dto.Rating > 5)
                errors.Add("Rating must be between 1 and 5");

            if (errors.Any())
                return ApiResponse<ReviewResponseDto>.FailureResponse(string.Join(", ", errors));

            var review = _mapper.Map<Reviews>(dto);
            await _unitOfWork.Reviews.InsertAsync(review);
            await _unitOfWork.SaveChangesAsync();
            InvalidateReviewCache();

            _logger.LogInformation("New review submitted by {ClientName}", dto.ClientName);
            var result = _mapper.Map<ReviewResponseDto>(review);
            return ApiResponse<ReviewResponseDto>.SuccessResponse(result, "Review submitted successfully. It will be visible after approval.");
        }

        public async Task<ApiResponse<bool>> ApproveReview(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null)
                return ApiResponse<bool>.FailureResponse("Review not found");

            if (review.IsApproved)
                return ApiResponse<bool>.SuccessResponse(true, "Review is already approved");

            review.IsApproved = true;
            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync();
            InvalidateReviewCache();

            return ApiResponse<bool>.SuccessResponse(true, "Review approved successfully");
        }

        public async Task<ApiResponse<PaginatedResult<ReviewResponseDto>>> GetAllReviews(ReviewFilterParams filters)
        {
            var query = _unitOfWork.Reviews.GetQueryable();
            if (filters.IsApproved.HasValue)
                query = query.Where(r => r.IsApproved == filters.IsApproved.Value);
            query = query.OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync();

            var pageNumber = filters.PageNumber < 1 ? 1 : filters.PageNumber;
            var pageSize = filters.PageSize < 1 ? 10 : filters.PageSize;

            var reviews = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!reviews.Any())
                return ApiResponse<PaginatedResult<ReviewResponseDto>>.SuccessResponse(
                    new PaginatedResult<ReviewResponseDto>
                    {
                        Data = new List<ReviewResponseDto>(),
                        CurrentPage = pageNumber,
                        TotalPages = 0,
                        TotalCount = 0,
                        PageSize = pageSize
                    }, "No reviews found");

            var mappedData = _mapper.Map<IEnumerable<ReviewResponseDto>>(reviews);

            var result = new PaginatedResult<ReviewResponseDto>
            {
                Data = mappedData,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                TotalCount = totalCount,
                PageSize = pageSize
            };

            return ApiResponse<PaginatedResult<ReviewResponseDto>>.SuccessResponse(
                result, "Reviews retrieved successfully");
        }

        public async Task<ApiResponse<ReviewResponseDto>> UpdateReview(int id, UpdateReviewDto dto)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null)
                return ApiResponse<ReviewResponseDto>.FailureResponse("Review not found");
            if (string.IsNullOrWhiteSpace(dto.Review))
                return ApiResponse<ReviewResponseDto>.FailureResponse("Review is required");
            if (dto.Rating < 1 || dto.Rating > 5)
                return ApiResponse<ReviewResponseDto>.FailureResponse("Rating must be between 1 and 5");

            _mapper.Map(dto, review);

            review.IsApproved = false;

            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync();
            InvalidateReviewCache();

            _logger.LogInformation("Review {Id} updated and reset to unapproved", id);
            var result = _mapper.Map<ReviewResponseDto>(review);
            return ApiResponse<ReviewResponseDto>.SuccessResponse(result, "Review updated successfully. It will be visible after re-approval.");
        }

        public async Task<ApiResponse<bool>> DeleteReview(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null)
                return ApiResponse<bool>.FailureResponse("Review not found");

            await _unitOfWork.Reviews.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            InvalidateReviewCache();

            return ApiResponse<bool>.SuccessResponse(true, "Review deleted successfully");
        }

        private void InvalidateReviewCache()
        {
            _cache.Remove(ApprovedCacheKey);
            _cache.Remove(AllReviewsCacheKey);
        }

    }
}
