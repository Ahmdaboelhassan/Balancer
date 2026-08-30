using Domain.DTO.Response;
using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;
public class AccountRepo : Repository<Account>, IAccountRepo
{
    private readonly AppDbContext _context;
    public AccountRepo(AppDbContext context) : base(context) => _context = context;
  
    public Task<LastAccountJournal?> GetLastAccountJournal(int accountId, int? costcenterId)
    {
       return _context.JournalDetails
            .Where(d => d.AccountId == accountId && (!costcenterId.HasValue || d.CostCenters.Any(cc => cc.CostCenterId == costcenterId.Value)))
            .OrderByDescending(d => d.Journal.Id)
            .Select(d => new LastAccountJournal
            {
                JournalId = d.JournalId,
                Type = d.Journal.Type,
                Name = d.Journal.Detail,
                Amount = Math.Max(d.Debit, d.Credit),
                JournalDate = d.Journal.CreatedAt,

                DebitAccount = d.Journal.JournalDetails
                    .Where(d => d.Debit > 0)
                    .Select(d => d.Account.Name)
                    .FirstOrDefault(),

                CreditAccount = d.Journal.JournalDetails
                    .Where(d => d.Credit > 0)
                    .Select(d => d.Account.Name)
                    .FirstOrDefault(),

                CostCenters = d.CostCenters
                        .Select(s => s.CostCenter.Name)

            }).FirstOrDefaultAsync();
    }
}
