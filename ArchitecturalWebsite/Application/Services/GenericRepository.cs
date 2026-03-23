using ArchitecturalWebsite.Application.Interfaces;
using ArchitecturalWebsite.Application.Services;
using ArchitecturalWebsite.Domain;
using ArchitecturalWebsite.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace ArchitecturalWebsite.Application.Services
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AWDbContext _dbSet;
        private DbSet<T> entities;

        public GenericRepository(AWDbContext dbContext)
        {
            _dbSet = dbContext;
            entities = dbContext.Set<T>();
        }

        public T Get(int id)
        {
            var entity = entities!.SingleOrDefault(x => x.Id == id);

            if (entity != null)
            {
                return entity;
            }

            return null;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return entities.AsEnumerable();
        }

        public IEnumerable<T> GetAllByCondition(Expression<Func<T, bool>>? expression = null, List<string>? includes = null)
        {
            IQueryable<T> query = entities;
            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            var result = query.AsNoTracking().AsEnumerable();
            return result;

        }

        public async Task<T?> GetByConditionAsync(Expression<Func<T, bool>> predicate, List<string>? includes = null)
        {
            IQueryable<T> query = entities;
            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public void InsertRange(List<T> entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.AddRange(entity);
            _dbSet.SaveChanges();
        }

        public async Task<int> Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            await entities.AddAsync(entity);
            return await _dbSet.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entities.Update(entity);
            _dbSet.SaveChanges();
        }

        public void UpdateRange(List<T> entity)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            _dbSet.UpdateRange(entity);
            _dbSet.SaveChanges();
        }

        public bool Delete(int id)
        {
            var entity = entities!.SingleOrDefault(x => x.Id == id);
            if (entity != null)
            {
                entities.Remove(entity);
                _dbSet.SaveChanges();
                return true;
            }
            return false;
        }


        public bool DeleteAll(List<T> entity)
        {

            if (entity == null)
            {
                return false;
            }
            _dbSet.RemoveRange(0, entity.Count);
            _dbSet.SaveChanges();
            return true;
        }

        public bool Remove(int Id)
        {
            var entity = entities!.SingleOrDefault(x => x.Id == Id);
            if (entity == null)
            {
                return false;
            }
            _dbSet.Remove(entity);
            return true;
        }

        public async Task<T> GetWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = entities;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = entities;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

    }
}