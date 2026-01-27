using Domain.DTO.Response;
using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    public async Task<Journal> GetNextJournal(string date)
    {
        if (!DateTime.TryParse(date, out DateTime d))
            return null;

        var nextJournal = await _context.Journals
            .Where(j => j.CreatedAt > d)
            .Include(j => j.JournalDetails)
            .OrderBy(j => j.CreatedAt)
            .ThenBy(j => j.Id)
            .FirstOrDefaultAsync();

        return nextJournal;
    }

    public async Task<Journal> GetPrevJournal(string date)
    {
        if (!DateTime.TryParse(date, out DateTime d))
            return null;

        var prevJournal = await _context.Journals
            .Where(j => j.CreatedAt < d)
            .Include(j => j.JournalDetails)
            .OrderByDescending(j => j.CreatedAt)
            .ThenByDescending(j => j.Id)
            .FirstOrDefaultAsync();

        return prevJournal;
    }
}
