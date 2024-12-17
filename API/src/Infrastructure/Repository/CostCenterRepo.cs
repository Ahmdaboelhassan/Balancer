using Application.IRepository;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repository;
public class CostCenterRepo : Repository<CostCenter>, ICostCenterRepo
{
    public CostCenterRepo(AppDbContext context) : base(context)
    {
    }
}
