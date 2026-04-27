using ArchitecturalWebsite.Application.DTOs;

namespace ArchitecturalWebsite.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<CategoryResponseDto>> CreateCategory(CreateCategoryDto createCategoryDto);
        Task<ApiResponse<IEnumerable<CategoryResponseDto>>> GetAllCategories();
        Task<ApiResponse<CategoryResponseDto>> GetCategoryById(int id);
    }
}
