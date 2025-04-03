using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace Infrastructure.Repository;
internal class SettingsRepo : Repository<Settings>, ISettingsRepo
{
    public SettingsRepo(AppDbContext context) : base(context)
    {
    }
}
