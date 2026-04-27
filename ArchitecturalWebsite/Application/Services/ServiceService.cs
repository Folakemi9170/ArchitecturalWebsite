using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Application.Interfaces;
using ArchitecturalWebsite.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;

namespace ArchitecturalWebsite.Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ServiceService> _logger;

        public ServiceService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment environment, ILogger<ServiceService> logger)
        { _unitOfWork = unitOfWork; _mapper = mapper; _environment = environment; _logger = logger; }

        public async Task<ApiResponse<ServiceDetailResponseDto>> CreateService(CreateServiceDto Dto)
        {
            if (string.IsNullOrWhiteSpace(Dto.Name))
                return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Service name is required");
            if (Dto.Price <= 0)
                return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Price must be greater than zero");
            var category = await _unitOfWork.Categories.GetByIdAsync(Dto.CategoryId);
            if (category == null)
                return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Category not found");
            var existing = await _unitOfWork.Services.GetByConditionAsync(s => s.Name == Dto.Name);
            if (existing != null)
                return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Service with this name already exists");
            if (Dto.ImageFile == null || Dto.ImageFile.Length == 0)
                return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Image is required");
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(Dto.ImageFile.ContentType))
                return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Invalid image type. Use JPEG, PNG, GIF, or WebP.");

            if (Dto.ImageFile.Length > 5 * 1024 * 1024)
                return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Image too large. Max 5MB.");

            string imageUrl;
            try
            {
                imageUrl = await SaveImageAsync(Dto.ImageFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save image for service {ServiceName}", Dto.Name);
                return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Failed to save image");
            }

            var service = _mapper.Map<Service>(Dto);
            service.ImageUrl = imageUrl;
            await _unitOfWork.Services.InsertAsync(service);
            await _unitOfWork.SaveChangesAsync();

            var createdService = await _unitOfWork.Services.GetByIdWithIncludesAsync(service.Id, s => s.Category);
            return ApiResponse<ServiceDetailResponseDto>.SuccessResponse(_mapper.Map<ServiceDetailResponseDto>(createdService),
                "Service created successfully");
        }

        public async Task<ApiResponse<IEnumerable<ServiceResponseDto>>> GetAllServices()
        {
            var services = await _unitOfWork.Services.GetAllWithIncludesAsync(s => s.Category);

            if (!services.Any())
                return ApiResponse<IEnumerable<ServiceResponseDto>>.FailureResponse("No services found");

            var mapped = _mapper.Map<IEnumerable<ServiceResponseDto>>(services);
            return ApiResponse<IEnumerable<ServiceResponseDto>>.SuccessResponse(mapped, "Services retrieved successfully");

        }

        public async Task<ApiResponse<ServiceDetailResponseDto>> GetServiceById(int id)
        {
            var service = await _unitOfWork.Services.GetByIdWithIncludesAsync(id, s => s.Category);
            if (service == null)
                return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Service not found");
            var mapped = _mapper.Map<ServiceDetailResponseDto>(service);
            return ApiResponse<ServiceDetailResponseDto>.SuccessResponse(mapped, "Service retrieved successfully");
        }

        public async Task<ApiResponse<ServicesByCategoryDto>> GetServicesByCategory(int categoryId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null)
                return ApiResponse<ServicesByCategoryDto>.FailureResponse("Category not found");
            var services = await _unitOfWork.Services.GetAllByConditionAsync( s => s.CategoryId == categoryId);

            var response = new ServicesByCategoryDto
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                CategoryDescription = category.Description,
                Services = _mapper.Map<IEnumerable<ServiceResponseDto>>(services)
            };

            return ApiResponse<ServicesByCategoryDto>.SuccessResponse(response, "Services retrieved successfully");
        }

        public async Task<ApiResponse<ServiceDetailResponseDto>> PatchService(int id, UpdateServiceDto dto)
        {
            var service = await _unitOfWork.Services.GetByIdWithIncludesAsync(id, s => s.Category);
            if (service == null)
                return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Service not found");

            if (dto.Price.HasValue && dto.Price.Value <= 0)
                return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Price must be greater than zero");

            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != service.Name)
            {
                var existing = await _unitOfWork.Services.GetByConditionAsync(s => s.Name == dto.Name && s.Id != id);
                if (existing != null)
                    return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Another service with this name already exists");
            }

            if (dto.CategoryId.HasValue && dto.CategoryId.Value != service.CategoryId)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId.Value);
                if (category == null)
                    return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Category not found");

                service.CategoryId = category.Id;
            }

            if (dto.ImageFile != null)
            {
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                if (!allowedTypes.Contains(dto.ImageFile.ContentType))
                    return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Invalid image type.");
                if (dto.ImageFile.Length > 5 * 1024 * 1024)
                    return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Image too large. Max 5MB.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
                service.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.ShortDescription))
                service.ShortDescription = dto.ShortDescription;
            if (!string.IsNullOrWhiteSpace(dto.FullDescription))
                service.FullDescription = dto.FullDescription;
            if (dto.Price.HasValue)
                service.Price = dto.Price.Value;

            string? oldImageUrl = null;
            if (dto.ImageFile != null)
            {
                oldImageUrl = service.ImageUrl;
                try
                {
                    service.ImageUrl = await SaveImageAsync(dto.ImageFile);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save new image for service {ServiceId}", id);
                    return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Failed to save image");
                }
            }

            try
            {
                _unitOfWork.Services.Update(service);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating service {ServiceId}", id);
                if (dto.ImageFile != null && !string.IsNullOrWhiteSpace(service.ImageUrl))
                    DeleteImage(service.ImageUrl);
                return ApiResponse<ServiceDetailResponseDto>.FailureResponse("Failed to update service.");
            }

            if (!string.IsNullOrWhiteSpace(oldImageUrl))
                DeleteImage(oldImageUrl);

            var updated = await _unitOfWork.Services.GetByIdWithIncludesAsync(id, s => s.Category);

            return ApiResponse<ServiceDetailResponseDto>.SuccessResponse( _mapper.Map<ServiceDetailResponseDto>(updated), "Service updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteService(int id)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(id);
            if (service == null)
                return ApiResponse<bool>.FailureResponse("Service not found");

            // Delete associated image
            if (!string.IsNullOrWhiteSpace(service.ImageUrl))
                DeleteImage(service.ImageUrl);

            try
            {
                var result = await _unitOfWork.Services.DeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.FailureResponse("Failed to delete service");
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Service deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting service {ServiceId}", id);
                return ApiResponse<bool>.FailureResponse("Error deleting service");
            }
        }



        //Helper class
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads", "services");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            _logger.LogInformation("Saved image: {FileName} ({Size} bytes)", fileName, imageFile.Length);

            return $"/uploads/services/{fileName}";
        }

        private void DeleteImage(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;

            try
            {
                var filePath = Path.Combine(_environment.WebRootPath ?? "wwwroot", imageUrl.TrimStart('/'));

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("Deleted image: {FilePath}", filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete image: {ImageUrl}", imageUrl);
            }
        }
    }
}
