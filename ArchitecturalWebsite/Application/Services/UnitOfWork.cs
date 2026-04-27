using ArchitecturalWebsite.Application.Interfaces;
using ArchitecturalWebsite.Domain;
using ArchitecturalWebsite.Infrastructure;
using System.Threading.Tasks;

namespace ArchitecturalWebsite.Application.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AWDbContext _context;

        public IGenericRepository<Service> Services { get; private set; }
        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Booking> Bookings { get; private set; }
        public IGenericRepository<Enquiry> Enquiries { get; private set; }
        public IGenericRepository<FAQ> FAQs { get; private set; }
        public IGenericRepository<Reviews> Reviews {  get; private set; }

        public UnitOfWork(AWDbContext Dbcontext)
        {
            _context = Dbcontext;
            Services = new GenericRepository<Service>(Dbcontext);
            Categories = new GenericRepository<Category>(Dbcontext);
            Bookings = new GenericRepository<Booking>(Dbcontext);
            Enquiries = new GenericRepository<Enquiry>(Dbcontext);
            FAQs = new GenericRepository<FAQ>(Dbcontext);
            Reviews = new GenericRepository<Reviews>(Dbcontext);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}