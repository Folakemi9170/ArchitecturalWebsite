namespace ArchitecturalWebsite.Domain
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
