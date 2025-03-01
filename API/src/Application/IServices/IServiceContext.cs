namespace Application.IServices;
public interface IServiceContext
{
    IPeriodService PeriodService { get; }
    IAccountService AccountService { get; }
    IJournalService JournalService { get; }
    ICostCenterService CostCenterService { get; }
    IHomeService HomeService { get; }
    IReportService ReportService { get; }
    IAuthService AuthService { get;}
  
}

