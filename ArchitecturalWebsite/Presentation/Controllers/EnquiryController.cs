using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArchitecturalWebsite.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnquiryController : ControllerBase
    {
        private readonly IEnquiryService _enquiryService;

        public EnquiryController(IEnquiryService enquiryService)
        {
            _enquiryService = enquiryService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnquiry([FromBody] CreateEnquiryDto dto)
        {
            var response = await _enquiryService.CreateEnquiry(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEnquiries([FromQuery] EnquiryFilterParams filters)
        {
            var response = await _enquiryService.GetAllEnquiries(filters);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnquiryById(int id)
        {
            var response = await _enquiryService.GetEnquiryById(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPut("{id}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var response = await _enquiryService.MarkAsRead(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}
