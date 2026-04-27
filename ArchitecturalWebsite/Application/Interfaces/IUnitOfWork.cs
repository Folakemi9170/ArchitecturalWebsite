using ArchitecturalWebsite.Domain;

namespace ArchitecturalWebsite.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Service> Services { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Booking> Bookings { get; }
        IGenericRepository<Enquiry> Enquiries { get; }
        IGenericRepository<FAQ> FAQs { get; }
        IGenericRepository<Reviews> Reviews { get; }
        Task<int> SaveChangesAsync();
    }
}
