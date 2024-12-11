using Application.IRepository;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repository;
public class AccountRepo : Repository<Account>, IAccountRepo
{
    public AccountRepo(AppDbContext context) : base(context)
    {
    }
}
