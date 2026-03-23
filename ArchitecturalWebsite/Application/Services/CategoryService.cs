using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Application.Interfaces;
using ArchitecturalWebsite.Domain;
using ArchitecturalWebsite.Infrastructure;
using AutoMapper;

namespace ArchitecturalWebsite.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IMapper _mapper;

        public CategoryService(IGenericRepository<Category> categoryRepo, IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CategoryResponseDto>> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            if (string.IsNullOrWhiteSpace(createCategoryDto.Name))
                return ApiResponse<CategoryResponseDto>.FailureResponse("Category name is required");

            var existing = await _categoryRepo.GetByConditionAsync(c => c.Name == createCategoryDto.Name);
            if (existing != null)
                return ApiResponse<CategoryResponseDto>.FailureResponse("Category already exists");

            var category = new Category
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepo.Insert(category);
            return ApiResponse<CategoryResponseDto>.SuccessResponse(
                    _mapper.Map<CategoryResponseDto>(category),
                    "Category created successfully"
                );
        }

        public async Task<ApiResponse<IEnumerable<CategoryResponseDto>>> GetAllCategories()
        {
            var categories = await _categoryRepo.GetAll();

            if (!categories.Any())
                return ApiResponse<IEnumerable<CategoryResponseDto>>.FailureResponse("No categories found");

            var result = _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
            return ApiResponse<IEnumerable<CategoryResponseDto>>.SuccessResponse(result, "Categories retrieved successfully");
        }

        public async Task<ApiResponse<CategoryResponseDto>> GetCategoryById(int id)
        {
            var category = await _categoryRepo.GetByConditionAsync(c => c.Id == id);

            if (category == null)
                return ApiResponse<CategoryResponseDto>.FailureResponse($"Category with id {id} not found");

            var result = _mapper.Map<CategoryResponseDto>(category);
            return ApiResponse<CategoryResponseDto>.SuccessResponse(result, "Category retrieved successfully");
        }
    }
}
