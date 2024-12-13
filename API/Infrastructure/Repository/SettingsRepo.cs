using Application.IRepository;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repository;
internal class SettingsRepo : Repository<Settings>, ISettingsRepo
{
    public SettingsRepo(AppDbContext context) : base(context)
    {
    }
}
