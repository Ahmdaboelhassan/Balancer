using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;


namespace Application.IRepository;
public interface IRepository<T>
{
    // Queries
    Task<T?> Get(int id);
    Task<T?> Get(Expression<Func<T, bool>> criteria, params string[] includes);
    Task<T?> GetFirst(params string[] includes);
    Task<T?> GetLast(Expression<Func<T, bool>> criteria, params string[] includes);
    Task<T?> GetLastOrderBy<Key>(Expression<Func<T, Key>> orderBy, params string[] includes);
    Task<List<T>> GetAll(params string[]? includes);
    Task<List<T>> GetAll(Expression<Func<T, bool>> criteria, params string[]? includes);
    Task<IEnumerable<O>> SelectAll<O>(Expression<Func<T, bool>> criteria, Expression<Func<T, O>> columns, params string[]? includes);
    Task<bool> Exists(Expression<Func<T, bool>> criteria);
    Task<int> Count(Expression<Func<T, bool>> criteria);

    // Command
    Task AddAsync(T element);
    void Delete(T element);
    void Update(T element);
    void DeleteRange(IEnumerable<T> items);
    void UpdateRange(IEnumerable<T> items);
    Task ExecuteUpdateAsync(Expression<Func<T, bool>> criteria, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> update);
}
