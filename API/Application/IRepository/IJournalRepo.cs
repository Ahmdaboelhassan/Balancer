using Domain.Models;

namespace Application.IRepository;
public interface IJournalRepo : IRepository<Journal>
{
    public int GetMaxCode();
}
