using System.Data;

namespace Application.IRepository;
public interface IUnitOfWork
{

    IPeriodRepo Periods { get; }
    IAccountRepo Accounts { get; }
     IJournalRepo Journal { get;}
     IJournalDetailRepo JournalDetail { get;}
     ICostCenterRepo CostCenter { get;}

    Task SaveChangesAync();
    IDbTransaction StartTransaction();
}
