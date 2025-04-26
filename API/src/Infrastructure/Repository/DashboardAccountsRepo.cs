using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace Infrastructure.Repository;
public class DashboardAccountsRepo : Repository<DashboardSettings>, IDashboardAccountsRepo
{
    public DashboardAccountsRepo(AppDbContext context) : base(context)
    {
    }
}
