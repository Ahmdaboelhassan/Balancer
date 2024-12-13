using Application.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Data;
using System.Linq.Expressions;

namespace Infrastructure.Repository;
public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbSet<T> _set;
    private readonly AppDbContext _context;

    public Repository(AppDbContext context) 
    {
        _context = context;
        _set = _context.Set<T>();
    }

    // Queries
    public  async Task<T?> Get(int id)
    {
        return await _set.FindAsync(id);
    }
    public Task<T?> Get(Expression<Func<T, bool>> criteria, params string[] includes)
    {
        var query = _set.AsNoTracking().Where(criteria);
        foreach (var item in includes)
            query = query.Include(item);

        return query.FirstOrDefaultAsync();
    }
    public Task<T?> GetFirst(params string[] includes)
    {
        var query = _set.AsNoTracking();
        foreach (var item in includes)
            query = query.Include(item);

        return query.FirstOrDefaultAsync();
    }
    public Task<T?> GetLast(params string[] includes)
    {
        var query = _set.AsNoTracking();
        foreach (var item in includes)
            query = query.Include(item);

        return query.LastOrDefaultAsync();
    }
    public Task<T?> GetLast(Expression<Func<T, bool>> criteria, params string[] includes)
    {
        var query = _set.AsNoTracking().Where(criteria); ;
        foreach (var item in includes)
            query = query.Include(item);

        return query.LastOrDefaultAsync();
    }
    public Task<List<T>> GetAll(Expression<Func<T, bool>> criteria, params string[]? includes)
    {
        var query = _set
            .Where(criteria)
            .AsNoTracking();

        foreach (var item in includes)
            query = query.Include(item);

        return query.ToListAsync();
    }
    public Task<List<T>> GetAll(params string[]? includes)
    {
        var query = _set.AsNoTracking();

        foreach (var item in includes)
            query = query.Include(item);

        return query.ToListAsync();
    }
    public async Task<IEnumerable<O>> SelectAll<O>(Expression<Func<T, bool>> criteria, Expression<Func<T, O>> columns, params string[]? includes)
    {
        var query = _set
            .Where(criteria)
            .AsNoTracking();

        foreach (var item in includes)
        {
            query = query.Include(item);
        }

        return await query
            .Select(columns)
            .ToListAsync();
    }
    public async Task<bool> Exists(Expression<Func<T, bool>> criteria)
    {
        return await _set.AnyAsync(criteria);
    }
    public Task<int> Count(Expression<Func<T, bool>> criteria)
    {
        return _set.CountAsync(criteria);
    }

    // Commands
    public async Task AddAsync(T element)
    {
        await _set.AddAsync(element);
    }
    public void Update(T element)
    {
        _set.Update(element);
    }
    public void Delete(T element)
    {
        _set.Remove(element);
    }
    public void DeleteRange(IEnumerable<T> items)
    {
        _set.RemoveRange(items);
    }
    public void UpdateRange(IEnumerable<T> items)
    {
        _set.UpdateRange(items);
    }
    public async Task ExecuteUpdateAsync(Expression<Func<T, bool>> criteria , Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> update)
    {
       await _set.Where(criteria).ExecuteUpdateAsync(update);
    }
}
