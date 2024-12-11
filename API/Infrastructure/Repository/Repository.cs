using Application.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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

    public Task<T> Get(Expression<Func<T, bool>> criteria, params string[] includes)
    {
        var query = _set.AsNoTracking().Where(criteria);
        foreach (var item in includes)
            query = query.Include(item);

        return query.FirstOrDefaultAsync();
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
    public async Task<bool> Exists(Expression<Func<T, bool>> criteria)
    {
        return await _set.AnyAsync(criteria);
    }
    public Task AddAsync(T element)
    {
        _set.AddAsync(element);
        return Task.CompletedTask;
    }
    public Task UpdateAsync(T element)
    {
        _set.Update(element);
        return Task.CompletedTask;
    }
    public Task DeleteAsync(T element)
    {
        _set.Remove(element);
        return Task.CompletedTask;
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
}
