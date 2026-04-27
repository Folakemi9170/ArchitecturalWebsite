using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Application.Interfaces;
using ArchitecturalWebsite.Domain;
using AutoMapper;

namespace ArchitecturalWebsite.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CategoryResponseDto>> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            if (string.IsNullOrWhiteSpace(createCategoryDto.Name))
                return ApiResponse<CategoryResponseDto>.FailureResponse("Category name is required");

            var existing = await _unitOfWork.Categories.GetByConditionAsync(c => c.Name == createCategoryDto.Name);
            if (existing != null)
                return ApiResponse<CategoryResponseDto>.FailureResponse("Category already exists");

            var category = _mapper.Map<Category>(createCategoryDto);

            await _unitOfWork.Categories.InsertAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<CategoryResponseDto>.SuccessResponse(
                    _mapper.Map<CategoryResponseDto>(category),
                    "Category created successfully"
                );
        }


        public async Task<ApiResponse<IEnumerable<CategoryResponseDto>>> GetAllCategories()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();

            if (!categories.Any())
                return ApiResponse<IEnumerable<CategoryResponseDto>>.FailureResponse("No categories found");

            var result = _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
            return ApiResponse<IEnumerable<CategoryResponseDto>>.SuccessResponse(result, "Categories retrieved successfully");
        }


        public async Task<ApiResponse<CategoryResponseDto>> GetCategoryById(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return ApiResponse<CategoryResponseDto>.FailureResponse($"Category with id {id} not found");

            var result = _mapper.Map<CategoryResponseDto>(category);
            return ApiResponse<CategoryResponseDto>.SuccessResponse(result, "Category retrieved successfully");
        }
    }
}
