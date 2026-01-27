using Domain.Entities;

namespace Domain.IRepository;
public interface IJournalRepo : IRepository<Journal>
{
    public int GetMaxCode();
    public Task<Journal> GetNextJournal(string date);
    public Task<Journal> GetPrevJournal(string date);
}
