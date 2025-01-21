using Application.IRepository;
using Application.IServices;
using Application.Models;
using Infrastructure.Repository;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;
public class ServiceContext : IServiceContext
{
    public ServiceContext(
           IPeriodService periodService,
           IAccountService accountService,
           IJournalService journalService,
           ICostCenterService costCenterService,
           IHomeService homeService,
           IReportService reportService,
           IAuthService authService,
           IPasswordHasher passwordHasher,
           ITokenService tokenService)
    {
        PeriodService = periodService;
        AccountService = accountService;
        JournalService = journalService;
        CostCenterService = costCenterService;
        HomeService = homeService;
        ReportService = reportService;
        AuthService = authService;
        PasswordHasher = passwordHasher;
        TokenService = tokenService;
    }
    public IPeriodService PeriodService { get; private set; }
    public IAccountService AccountService { get; private set; }
    public IJournalService JournalService { get; private set; }
    public ICostCenterService CostCenterService { get; private set; }
    public IHomeService HomeService { get; private set; }
    public IReportService ReportService { get; private set; }
    public IAuthService AuthService { get; private set; }
    public IPasswordHasher PasswordHasher { get; private set; }
    public ITokenService TokenService{ get; private set; }
}
