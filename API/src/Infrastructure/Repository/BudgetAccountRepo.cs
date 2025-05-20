using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace Infrastructure.Repository
{
    public class BudgetAccountRepo : Repository<BudgetAccount>, IBudgetAccountRepo
    {
        public BudgetAccountRepo(AppDbContext context) : base(context)
        {
        }
    }
   
}
