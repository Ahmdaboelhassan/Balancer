using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace Infrastructure.Repository;
public class JournalDetailRepo : Repository<JournalDetail>, IJournalDetailRepo
{
    public JournalDetailRepo(AppDbContext context) : base(context)
    {
    }
}
