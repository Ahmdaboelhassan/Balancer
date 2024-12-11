using Application.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Infrastructure.Repository;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IPeriodRepo Periods { get; private set; }
    public IAccountRepo Accounts { get; private set; }
    public IJournalRepo Journal { get; private set; }
    public IJournalDetailRepo JournalDetail { get; private set; }
    public ICostCenterRepo CostCenter { get; private set; }
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Periods = new PeriodRepo(context);
        Accounts = new AccountRepo(context);
        Journal = new JournalRepo(context);
        JournalDetail = new JournalDetailRepo(context);
        CostCenter = new CostCenterRepo(context);
    }
   

    public Task SaveChangesAync()
    {
        return _context.SaveChangesAsync();
    }

    public IDbTransaction StartTransaction()
    {
        var transaction = _context.Database.BeginTransaction();
        return transaction.GetDbTransaction();
    }
}
