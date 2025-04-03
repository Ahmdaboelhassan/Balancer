using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace Infrastructure.Repository;
public class PeriodRepo : Repository<Period>, IPeriodRepo
{
    public PeriodRepo(AppDbContext context) : base(context)
    {
    }
}
