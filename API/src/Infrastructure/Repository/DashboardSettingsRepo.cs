using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace Infrastructure.Repository;
public class DashboardSettingsRepo : Repository<DashboardSettings>, IDashboardSettingsRepo
{
    public DashboardSettingsRepo(AppDbContext context) : base(context)
    {
    }
}
