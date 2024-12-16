using Application.IRepository;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repository;
internal class JournalRepo : Repository<Journal>, IJournalRepo
{
    private readonly AppDbContext _context;
    public JournalRepo(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public int GetMaxCode()
    {
        return _context.Journals.Max(x => x.Code);
    }
}
