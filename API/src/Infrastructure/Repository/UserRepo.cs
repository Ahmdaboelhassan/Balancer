using Application.IRepository;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repository;
public class UserRepo : Repository<User>, IUserRepo
{
    public UserRepo(AppDbContext context) : base(context)
    {
    }
}
