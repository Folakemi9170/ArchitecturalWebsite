namespace ArchitecturalWebsite.Application.DTOs
{
    public class ReviewDto
    {
        public class CreateReviewDto
        {
            public string ClientName { get; set; }
            public string Review { get; set; }
            public int Rating { get; set; }
        }

        public class ReviewFilterParams : PaginationParams
        {
            public bool? IsApproved { get; set; }
        }

        public class ReviewResponseDto
        {
            public int Id { get; set; }
            public string ClientName { get; set; }
            public string Review { get; set; }
            public int Rating { get; set; }
            public bool IsApproved { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class UpdateReviewDto
        {
            public string Review { get; set; }
            public int Rating { get; set; }
        }
    }
}
