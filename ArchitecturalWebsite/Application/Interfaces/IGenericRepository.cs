using ArchitecturalWebsite.Domain;
using System.Linq.Expressions;

namespace ArchitecturalWebsite.Application.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<T?> GetByConditionAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllByConditionAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task InsertAsync(T entity);
        Task InsertRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void DeleteRange(IEnumerable<T> entities);
        void UpdateRange(IEnumerable<T> entities);
        Task<bool> DeleteAsync(int id);
        IQueryable<T> GetQueryable();

        //Task AddAsync(T entity);
        //void AddRange(List<T> entities);
    }
}
