using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace Infrastructure.Repository
{
    public class EvaluationDetailRepo : Repository<EvaluationDetail>, IEvaluationDetailRepo
    {
        public EvaluationDetailRepo(AppDbContext context) : base(context)
        {
        }
    }
}
