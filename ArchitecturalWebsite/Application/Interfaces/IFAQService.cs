using ArchitecturalWebsite.Application.DTOs;
using static ArchitecturalWebsite.Application.DTOs.FAQDto;

namespace ArchitecturalWebsite.Application.Interfaces
{
    public interface IFAQService
    {
        Task<ApiResponse<FAQResponseDto>> CreateFAQ(CreateFAQDto dto);
        Task<ApiResponse<IEnumerable<FAQResponseDto>>> GetAllFAQs();
        Task<ApiResponse<FAQResponseDto>> GetFAQById(int id);
        Task<ApiResponse<FAQResponseDto>> UpdateFAQ(int id, UpdateFAQDto dto);
        Task<ApiResponse<bool>> DeleteFAQ(int id);
    }
}
