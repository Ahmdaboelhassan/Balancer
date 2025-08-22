using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace Infrastructure.Repository
{
    public class EvaluationRepo : Repository<Evaluation>, IEvaluationRepo
    {
        public EvaluationRepo(AppDbContext context) : base(context)
        {
        }
    }
}
