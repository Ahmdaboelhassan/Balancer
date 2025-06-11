using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Infrastructure.Repository;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IPeriodRepo Periods { get;}
    public IAccountRepo Accounts { get;}
    public IJournalRepo Journal { get;}
    public IJournalDetailRepo JournalDetail { get;}
    public ICostCenterRepo CostCenter { get;}
    public ISettingsRepo Settings { get;}
    public IUserRepo Users { get;}
    public IBudgetAccountRepo BudgetAccounts { get;}
    public IDashboardSettingsRepo DashboardSettings { get;}
    public IRefreshTokenRepo RefreshTokens { get;}

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Periods = new PeriodRepo(context);
        Accounts = new AccountRepo(context);
        Journal = new JournalRepo(context);
        JournalDetail = new JournalDetailRepo(context);
        CostCenter = new CostCenterRepo(context);
        Settings = new SettingsRepo(context);
        Users = new UserRepo(context);
        BudgetAccounts = new BudgetAccountRepo(context);
        DashboardSettings = new DashboardSettingsRepo(context);
        RefreshTokens = new RefreshTokenRepo(context);
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
