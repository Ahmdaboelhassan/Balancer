using Application.IRepository;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repository;
public class JournalDetailRepo : Repository<JournalDetail>, IJournalDetailRepo
{
    public JournalDetailRepo(AppDbContext context) : base(context)
    {
    }
}
