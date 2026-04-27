using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArchitecturalWebsite.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpPost("with-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateService([FromForm] CreateServiceDto Dto)
        {
            var result = await _serviceService.CreateService(Dto);

            if (!result.Success)
                return BadRequest(result);
            return StatusCode(201, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var result = await _serviceService.GetAllServices();

            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var result = await _serviceService.GetServiceById(id);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetServicesByCategory(int categoryId)
        {
            var result = await _serviceService.GetServicesByCategory(categoryId);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateService(int id, [FromForm] UpdateServiceDto dto)
        {
            var result = await _serviceService.PatchService(id, dto);

            if (!result.Success)
                return result.Message?.Contains("not found") == true ? NotFound(result) : BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var result = await _serviceService.DeleteService(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
