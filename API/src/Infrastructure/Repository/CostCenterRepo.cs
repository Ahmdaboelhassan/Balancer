using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace Infrastructure.Repository;
public class CostCenterRepo : Repository<CostCenter>, ICostCenterRepo
{
    public CostCenterRepo(AppDbContext context) : base(context)
    {
    }
}
