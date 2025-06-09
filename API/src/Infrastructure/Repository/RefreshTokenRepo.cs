using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace Infrastructure.Repository;

public class RefreshTokenRepo : Repository<RefreshToken>, IRefreshTokenRepo
{
    public RefreshTokenRepo(AppDbContext context) : base(context)
    {
    }
}
