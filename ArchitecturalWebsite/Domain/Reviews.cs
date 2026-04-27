namespace ArchitecturalWebsite.Domain
{
    public class Reviews : BaseEntity
    {
        public string ClientName { get; set; }
        public string Review { get; set; }
        public int Rating { get; set; }
        public bool IsApproved { get; set; }
    }
}
