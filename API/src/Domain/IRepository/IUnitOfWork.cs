using System.Data;

namespace Domain.IRepository;
public interface IUnitOfWork
{

    IPeriodRepo Periods { get; }
    IAccountRepo Accounts { get; }
     IJournalRepo Journal { get;}
     IJournalDetailRepo JournalDetail { get;}
     ICostCenterRepo CostCenter { get;}
     ISettingsRepo Settings { get; }
     IUserRepo Users{ get; }
    IDashboardSettingsRepo DashboardSettings { get; }
    IBudgetAccountRepo BudgetAccounts { get; }
    IRefreshTokenRepo RefreshTokens { get; }
    Task SaveChangesAync();
    IDbTransaction StartTransaction();
}
