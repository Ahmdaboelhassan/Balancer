using Application.IRepository;
using Application.IServices;

namespace Infrastructure.Services;
public class ServiceContext : IServiceContext
{
    public ServiceContext(IUnitOfWork uow)
    {
        PeriodService = new PeriodService(uow);
        AccountService = new AccountService(uow);
        JournalService = new JournalService(uow);
        CostCenterService = new CostCenterService(uow);
    }
    public IPeriodService PeriodService { get; private set; }
    public IAccountService AccountService { get; private set; }
    public IJournalService JournalService { get; private set; }
    public ICostCenterService CostCenterService { get; private set; }
}
