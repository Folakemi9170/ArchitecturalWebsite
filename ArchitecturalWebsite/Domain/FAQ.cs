namespace ArchitecturalWebsite.Domain
{
    public class FAQ : BaseEntity
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Category { get; set; }
        public int DisplayOrder { get; set; }
    }
}
