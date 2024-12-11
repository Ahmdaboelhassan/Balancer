using System.Linq.Expressions;

namespace Application.IRepository;
public interface IRepository<T>
{
    // Queries
    Task<T> Get(Expression<Func<T, bool>> criteria, params string[] includes);
    Task<List<T>> GetAll(params string[]? includes);
    Task<List<T>> GetAll(Expression<Func<T, bool>> criteria, params string[]? includes);
    Task<IEnumerable<O>> SelectAll<O>(Expression<Func<T, bool>> criteria, Expression<Func<T, O>> columns, params string[]? includes);
    Task<bool> Exists(Expression<Func<T, bool>> criteria);

    // Command
    Task AddAsync(T element);
    Task DeleteAsync(T element);
    Task UpdateAsync(T element);
}
