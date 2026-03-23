using ArchitecturalWebsite.Domain;
using System.Linq.Expressions;

namespace ArchitecturalWebsite.Application.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAll();
        IEnumerable<T> GetAllByCondition(Expression<Func<T, bool>>? expression = null, List<string>? includes = null);
        Task<T?> GetByConditionAsync(Expression<Func<T, bool>> predicate, List<string>? includes = null);
        T Get(int id);
        Task<int> Insert(T entity);
        void InsertRange(List<T> entity);
        void Update(T entity);
        void UpdateRange(List<T> entity);
        bool Delete(int id);
        bool DeleteAll(List<T> entity);
        bool Remove(int Id);
        Task<T> GetWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes);
        public Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
    }
}
