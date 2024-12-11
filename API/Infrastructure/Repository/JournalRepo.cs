using Application.IRepository;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repository;
internal class JournalRepo : Repository<Journal>, IJournalRepo
{
    public JournalRepo(AppDbContext context) : base(context)
    {
    }
}
