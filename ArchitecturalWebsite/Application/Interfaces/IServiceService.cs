using ArchitecturalWebsite.Application.DTOs;

namespace ArchitecturalWebsite.Application.Interfaces
{
    public interface IServiceService
    {
        Task<ApiResponse<ServiceDetailResponseDto>> CreateService(CreateServiceDto createServiceDto);
        Task<ApiResponse<IEnumerable<ServiceResponseDto>>> GetAllServices();
        Task<ApiResponse<ServicesByCategoryDto>> GetServicesByCategory(int categoryId);
        Task<ApiResponse<ServiceDetailResponseDto>> GetServiceById(int id);
        Task<ApiResponse<ServiceDetailResponseDto>> PatchService(int id, UpdateServiceDto updateServiceDto);
        Task<ApiResponse<bool>> DeleteService(int id);
    }
}

