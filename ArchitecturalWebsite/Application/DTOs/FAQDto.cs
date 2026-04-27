namespace ArchitecturalWebsite.Application.DTOs
{
    public class FAQDto
    {
        public class CreateFAQDto
        {
            public string Question { get; set; }
            public string Answer { get; set; }
            public string Category { get; set; }
            public int DisplayOrder { get; set; }
        }

        public class UpdateFAQDto
        {
            public string Question { get; set; }
            public string Answer { get; set; }
            public string Category { get; set; }
            public int DisplayOrder { get; set; }
        }

        public class FAQResponseDto
        {
            public int Id { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
            public string Category { get; set; }
            public int DisplayOrder { get; set; }
        }
    }
}
