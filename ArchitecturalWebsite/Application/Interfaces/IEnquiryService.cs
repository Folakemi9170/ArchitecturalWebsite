using ArchitecturalWebsite.Application.DTOs;

namespace ArchitecturalWebsite.Application.Interfaces
{
    public interface IEnquiryService
    {
        Task<ApiResponse<bool>> CreateEnquiry(CreateEnquiryDto dto);
        Task<ApiResponse<PaginatedResult<EnquiryResponseDto>>> GetAllEnquiries(EnquiryFilterParams filters);
        Task<ApiResponse<EnquiryResponseDto>> GetEnquiryById(int id);
        Task<ApiResponse<bool>> MarkAsRead(int id);
    }
}
