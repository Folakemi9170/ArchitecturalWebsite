using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Domain;
using ArchitecturalWebsite.Infrastructure;
using Microsoft.Extensions.Caching.Memory;

namespace ArchitecturalWebsite.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<CategoryResponseDto>> CreateCategory(CreateCategoryDto createCategoryDto);
        Task<ApiResponse<IEnumerable<CategoryResponseDto>>> GetAllCategories();
        Task<ApiResponse<CategoryResponseDto>> GetCategoryById(int id);
    }
}
