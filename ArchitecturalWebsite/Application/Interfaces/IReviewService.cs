using ArchitecturalWebsite.Application.DTOs;
using static ArchitecturalWebsite.Application.DTOs.ReviewDto;

namespace ArchitecturalWebsite.Application.Interfaces
{
    public interface IReviewService
    {
        Task<ApiResponse<ReviewResponseDto>> CreateReview(CreateReviewDto dto);
        Task<ApiResponse<bool>> ApproveReview(int id);
        Task<ApiResponse<PaginatedResult<ReviewResponseDto>>> GetAllReviews(ReviewFilterParams filters);
        Task<ApiResponse<ReviewResponseDto>> UpdateReview(int id, UpdateReviewDto dto);
        Task<ApiResponse<bool>> DeleteReview(int id);
    }
}
