namespace ArchitecturalWebsite.Domain
{
    public class Service : BaseEntity
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
