using ArchitecturalWebsite.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArchitecturalWebsite.Infrastructure
{
    public class AWDbContext: DbContext
    {
        public AWDbContext(DbContextOptions<AWDbContext> options) : base(options) { }

        public DbSet<Service> Services {get; set;}
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<Blogs> Blogs { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
