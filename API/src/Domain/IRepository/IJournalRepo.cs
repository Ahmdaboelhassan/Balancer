using Domain.Entities;

namespace Domain.IRepository;
public interface IJournalRepo : IRepository<Journal>
{
    public int GetMaxCode();
}
