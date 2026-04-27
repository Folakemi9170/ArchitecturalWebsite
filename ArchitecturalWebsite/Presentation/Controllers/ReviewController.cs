using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ArchitecturalWebsite.Application.DTOs.ReviewDto;

namespace ArchitecturalWebsite.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var response = await _reviewService.CreateReview(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveReview(int id)
        {
            var response = await _reviewService.ApproveReview(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews([FromQuery] ReviewFilterParams filters)
        {
            var response = await _reviewService.GetAllReviews(filters);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDto dto)
        {
            var response = await _reviewService.UpdateReview(id, dto);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var response = await _reviewService.DeleteReview(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}
