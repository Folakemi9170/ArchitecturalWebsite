using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Application.Interfaces;
using ArchitecturalWebsite.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static ArchitecturalWebsite.Application.DTOs.FAQDto;

namespace ArchitecturalWebsite.Application.Services
{
    public class FAQService : IFAQService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FAQService> _logger;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "all_faqs";

        public FAQService(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<FAQService> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<ApiResponse<FAQResponseDto>> CreateFAQ(CreateFAQDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Question))
                return ApiResponse<FAQResponseDto>.FailureResponse("Question is required");
            if (string.IsNullOrWhiteSpace(dto.Answer))
                return ApiResponse<FAQResponseDto>.FailureResponse("Answer is required");
            if (string.IsNullOrWhiteSpace(dto.Category))
                return ApiResponse<FAQResponseDto>.FailureResponse("Category is required");

            var faq = _mapper.Map<FAQ>(dto);
            await _unitOfWork.FAQs.InsertAsync(faq);
            await _unitOfWork.SaveChangesAsync();
            InvalidateFAQCache();

            _logger.LogInformation("FAQ created: {Question}", dto.Question);
            var result = _mapper.Map<FAQResponseDto>(faq);
            return ApiResponse<FAQResponseDto>.SuccessResponse(result, "FAQ created successfully");
        }

        public async Task<ApiResponse<IEnumerable<FAQResponseDto>>> GetAllFAQs()
        {
            if (_cache.TryGetValue(CacheKey, out IEnumerable<FAQResponseDto> cachedFAQs))
            {
                _logger.LogInformation("Returning FAQs from cache");
                return ApiResponse<IEnumerable<FAQResponseDto>>.SuccessResponse(
                    cachedFAQs, "FAQs retrieved successfully");
            }

            var faqs = await _unitOfWork.FAQs.GetQueryable()
                .OrderBy(f => f.Category)
                .ThenBy(f => f.DisplayOrder)
                .ToListAsync();

            if (!faqs.Any())
                return ApiResponse<IEnumerable<FAQResponseDto>>.SuccessResponse(
                    new List<FAQResponseDto>(), "No FAQs found");

            var result = _mapper.Map<IEnumerable<FAQResponseDto>>(faqs);
            _cache.Set(CacheKey, result, TimeSpan.FromMinutes(10));

            return ApiResponse<IEnumerable<FAQResponseDto>>.SuccessResponse(
                result, "FAQs retrieved successfully");
        }

        public async Task<ApiResponse<FAQResponseDto>> GetFAQById(int id)
        {
            var faq = await _unitOfWork.FAQs.GetByIdAsync(id);
            if (faq == null)
                return ApiResponse<FAQResponseDto>.FailureResponse("FAQ not found");

            var result = _mapper.Map<FAQResponseDto>(faq);
            return ApiResponse<FAQResponseDto>.SuccessResponse(result, "FAQ retrieved successfully");
        }

        public async Task<ApiResponse<FAQResponseDto>> UpdateFAQ(int id, UpdateFAQDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Question))
                return ApiResponse<FAQResponseDto>.FailureResponse("Question is required");
            if (string.IsNullOrWhiteSpace(dto.Answer))
                return ApiResponse<FAQResponseDto>.FailureResponse("Answer is required");
            if (string.IsNullOrWhiteSpace(dto.Category))
                return ApiResponse<FAQResponseDto>.FailureResponse("Category is required");

            var faq = await _unitOfWork.FAQs.GetByIdAsync(id);
            if (faq == null)
                return ApiResponse<FAQResponseDto>.FailureResponse("FAQ not found");

            _mapper.Map(dto, faq);
            _unitOfWork.FAQs.Update(faq);
            await _unitOfWork.SaveChangesAsync();
            InvalidateFAQCache();

            var result = _mapper.Map<FAQResponseDto>(faq);
            return ApiResponse<FAQResponseDto>.SuccessResponse(result, "FAQ updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteFAQ(int id)
        {
            var faq = await _unitOfWork.FAQs.GetByIdAsync(id);
            if (faq == null)
                return ApiResponse<bool>.FailureResponse("FAQ not found");

            await _unitOfWork.FAQs.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            InvalidateFAQCache();

            return ApiResponse<bool>.SuccessResponse(true, "FAQ deleted successfully");
        }

        private void InvalidateFAQCache()
        {
            _cache.Remove(CacheKey);
        }
    }
}
