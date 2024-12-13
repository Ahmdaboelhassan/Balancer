using Application.IRepository;
using Application.IServices;

namespace Infrastructure.Services;
public class ServiceContext : IServiceContext
{
    public ServiceContext(IUnitOfWork uow)
    {
        PeriodService = new PeriodService(uow);
        AccountService = new AccountService(uow);
    }
    public IPeriodService PeriodService { get; private set; }
    public IAccountService AccountService { get; private set; }
}
