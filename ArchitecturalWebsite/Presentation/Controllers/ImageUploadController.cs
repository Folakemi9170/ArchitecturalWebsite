//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace ArchitecturalWebsite.Presentation.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UploadController : ControllerBase
//    {
//        private readonly IWebHostEnvironment _environment;
//        private readonly ILogger<UploadController> _logger;

//        public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger)
//        {
//            _environment = environment;
//            _logger = logger;
//        }

//        [HttpPost("image")]
//        public async Task<IActionResult> UploadImage(IFormFile file)
//        {
//            if (file == null || file.Length == 0)
//                return BadRequest("No file uploaded");

//            // Validate file type
//            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
//            if (!allowedTypes.Contains(file.ContentType))
//                return BadRequest("Invalid file type. Only JPEG, PNG, GIF, WebP allowed.");

//            // Validate file size (max 10MB)
//            const int maxSize = 10 * 1024 * 1024;
//            if (file.Length > maxSize)
//                return BadRequest("File too large. Max 5MB.");

//            // Create uploads directory
//            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "services");
//            if (!Directory.Exists(uploadsFolder))
//                Directory.CreateDirectory(uploadsFolder);

//            // Generate unique filename
//            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
//            var filePath = Path.Combine(uploadsFolder, fileName);

//            // Save file
//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

//            // Return URL
//            var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/services/{fileName}";

//            return Ok(new { imageUrl });
//        }
//    }
//}

