using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace Infrastructure.Repository;
public class JournalDetailCostCentersRepo : Repository<JournalDetailCostCenter>, IJournalDetailCostCentersRepo
{
    public JournalDetailCostCentersRepo(AppDbContext context) : base(context)
    {
    }
}
