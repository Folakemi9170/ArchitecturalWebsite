namespace ArchitecturalWebsite.Domain
{
    public class Blogs : BaseEntity
    {
        public string Title { get; set; } // "Eg. Top 10 Interior Design Trends for 2026"
        public string Slug { get; set; } // "eg-top-10-interior-design-trends-for-2026"
        public string Summary { get; set; } //A short preview of the article — shown on the blog list page before the reader clicks in.
        public string Content { get; set; } // The full article content, which can include text, images, and other media. This is what readers will see when they click on the blog post.
        public string ThumbnailUrl { get; set; } // A URL to an image that represents the blog post. This is often displayed alongside the title and summary on the blog list page.
        public string Author { get; set; } // The name of the person who wrote the blog post. This can be displayed on the blog post page and in the blog list.
        public bool IsPublished { get; set; } // A flag to indicate whether the blog post is published and should be visible to readers. This allows you to save drafts of blog posts that are not yet ready for publication.
        public DateTime? PublishedAt { get; set; } // The date and time when the blog post was published. This can be used to sort blog posts by publication date and to display the publication date on the blog post page.
    }
}
