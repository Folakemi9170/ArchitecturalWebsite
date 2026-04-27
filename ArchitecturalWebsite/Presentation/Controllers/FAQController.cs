using ArchitecturalWebsite.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ArchitecturalWebsite.Application.DTOs.FAQDto;

namespace ArchitecturalWebsite.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FAQController : ControllerBase
    {
        private readonly IFAQService _faqService;

        public FAQController(IFAQService faqService)
        {
            _faqService = faqService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFAQ([FromBody] CreateFAQDto dto)
        {
            var response = await _faqService.CreateFAQ(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFAQs()
        {
            var response = await _faqService.GetAllFAQs();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFAQById(int id)
        {
            var response = await _faqService.GetFAQById(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFAQ(int id, [FromBody] UpdateFAQDto dto)
        {
            var response = await _faqService.UpdateFAQ(id, dto);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFAQ(int id)
        {
            var response = await _faqService.DeleteFAQ(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}
