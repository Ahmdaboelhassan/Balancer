namespace Application.IServices;
public interface IServiceContext
{
    IPeriodService PeriodService { get; }
    IAccountService AccountService { get; }
}
