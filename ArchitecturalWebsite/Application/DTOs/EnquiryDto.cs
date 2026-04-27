namespace ArchitecturalWebsite.Application.DTOs
{
    public class CreateEnquiryDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

    public class EnquiryResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EnquiryFilterParams : PaginationParams
    {
        public bool? IsRead { get; set; }
    }
}
