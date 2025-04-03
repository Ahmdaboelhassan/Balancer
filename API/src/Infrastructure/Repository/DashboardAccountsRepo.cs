using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace Infrastructure.Repository;
public class DashboardAccountsRepo : Repository<DashboardAccounts>, IDashboardAccountsRepo
{
    public DashboardAccountsRepo(AppDbContext context) : base(context)
    {
    }
}
