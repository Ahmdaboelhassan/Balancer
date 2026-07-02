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
    public IPeriodRepo Periods { get => new PeriodRepo(_context); }
    public IAccountRepo Accounts { get => new AccountRepo(_context); }
    public IJournalRepo Journal { get => new JournalRepo(_context); }
    public IJournalDetailRepo JournalDetail { get => new JournalDetailRepo(_context); }
    public ICostCenterRepo CostCenter { get => new CostCenterRepo(_context); }
    public ISettingsRepo Settings { get => new SettingsRepo(_context); }
    public IUserRepo Users { get => new UserRepo(_context); }
    public IBudgetAccountRepo BudgetAccounts { get => new BudgetAccountRepo(_context); }
    public IDashboardSettingsRepo DashboardSettings { get => new DashboardSettingsRepo(_context); }
    public IRefreshTokenRepo RefreshTokens { get => new RefreshTokenRepo(_context); }
    public IEvaluationRepo Evaluations { get => new EvaluationRepo(_context); }
    public IEvaluationDetailRepo EvaluationDetails { get => new EvaluationDetailRepo(_context); }
    public IJournalDetailCostCentersRepo JournalDetailCostCenters { get => new JournalDetailCostCentersRepo(_context); }

    public UnitOfWork(AppDbContext context)
    {
       _context = context;
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
