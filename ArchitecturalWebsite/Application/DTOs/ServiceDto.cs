namespace ArchitecturalWebsite.Application.DTOs
{
    //for reading before you click on item/ getall
    public class ServiceResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
    }

    //for reading the full details of service items- get by id
    public class ServiceDetailResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public CategoryResponseDto Category { get; set; }
    }

    public class CreateServiceDto
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public IFormFile ImageFile { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }

    public class UpdateServiceDto
    {
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }
        public IFormFile? ImageFile { get; set; }
        public decimal? Price { get; set; }
        public int? CategoryId { get; set; }
    }

    public class ServicesByCategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryDescription { get; set; }
        public IEnumerable<ServiceResponseDto> Services { get; set; } = new List<ServiceResponseDto>();
    }
}
