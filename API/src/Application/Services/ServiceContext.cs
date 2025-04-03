using Domain.IServices;

namespace Application.Services;
public class ServiceContext : IServiceContext
{
    public ServiceContext(
           IPeriodService periodService,
           IAccountService accountService,
           IJournalService journalService,
           ICostCenterService costCenterService,
           IHomeService homeService,
           IReportService reportService,
           IAuthService authService
           )
    {
        PeriodService = periodService;
        AccountService = accountService;
        JournalService = journalService;
        CostCenterService = costCenterService;
        HomeService = homeService;
        ReportService = reportService;
        AuthService = authService;
    }
    public IPeriodService PeriodService { get; private set; }
    public IAccountService AccountService { get; private set; }
    public IJournalService JournalService { get; private set; }
    public ICostCenterService CostCenterService { get; private set; }
    public IHomeService HomeService { get; private set; }
    public IReportService ReportService { get; private set; }
    public IAuthService AuthService { get; private set; }
}
