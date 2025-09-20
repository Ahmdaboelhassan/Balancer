using Domain.DTO.Response;
using Domain.IRepository;
using Domain.IServices;
using Domain.Enums;
using System.Globalization;

namespace Application.Services;
public class ReportService : IReportService
{
    private readonly IUnitOfWork _uow;
    public ReportService(IUnitOfWork uow) => _uow = uow;

    public async Task<AccountStatement> GetAccountStatement(DateTime? from, DateTime? to, int accountId, int? costCenterId, bool openingBalance)
    {
        var account = await _uow.Accounts.Get(accountId);
        if (account == null)
            return GetEmptyAccountStatement();

        var journals = (await _uow.JournalDetail
                   .GetAll(d =>
                        d.Account.Number.StartsWith(account.Number)
                     && (!from.HasValue || d.Journal.CreatedAt.Date >= from.Value.Date)
                     && (!to.HasValue || d.Journal.CreatedAt.Date <= to.Value.Date)
                     && (!costCenterId.HasValue || d.CostCenterId == costCenterId)
                   , "CostCenter", "Account", "Journal")).OrderBy(d => d.Journal.CreatedAt);


        var journalsLinkedList = new LinkedList<AccountStatementDetail>();

        decimal balance = 0;
        if (openingBalance && from.HasValue)
        {
            var openingJournals = await _uow.JournalDetail
                    .GetAll(d =>
                         d.Account.Number.StartsWith(account.Number)
                       && d.Journal.CreatedAt < from.Value.Date
                      && (!costCenterId.HasValue ||  d.CostCenterId == costCenterId )
                    , "Account", "Journal");

            if (openingJournals.Count > 0)
            {
                balance = openingJournals.Sum(j => j.Debit - j.Credit);

                journalsLinkedList.AddFirst(new AccountStatementDetail
                {
                    Balance = balance,
                    Credit = openingJournals.Sum(j => j.Credit),
                    Debit = openingJournals.Sum(j => j.Debit),
                    Detail = "Opening Balance",
                    notes = "Opening Balance",
                    Date = openingJournals.First().Journal.CreatedAt.ToShortDateString(),
                });
            }
        }

        foreach (var journal in journals)
        {
            balance += journal.Debit - journal.Credit;

            journalsLinkedList.AddLast( new AccountStatementDetail
            {
                Balance = balance,
                Credit = journal.Credit,
                Debit = journal.Debit,
                CostCenter = journal.CostCenter?.Name ?? "",
                Detail = journal.Journal.Detail,
                JournalId = journal.JournalId,
                notes = journal.Journal.Notes,
                PeriodId = journal.Journal.PeriodId,
                Date = journal.Journal.CreatedAt.ToShortDateString(),
            });
        }

        if (journalsLinkedList.Count > 0)
        {
            journalsLinkedList.AddLast(new AccountStatementDetail
            {
                Balance = balance,
                Credit = journalsLinkedList.Sum(j => j.Credit),
                Debit = journalsLinkedList.Sum(j => j.Debit),
                CostCenter = "",
                Detail = "Total",
                JournalId = 0,
                notes = "Total",
                PeriodId = 0,
                Date = "",
            });
        }
        return new AccountStatement()
        {
            AccountType = balance > 0 ? "Debit" : "Credit",
            Details = journalsLinkedList,
            AccountName = account.Name,
            From = from?.ToString("d") ?? "",
            To = to?.ToString("d") ?? "",
            Amount = Math.Abs(balance).ToString("c")
        };
    }

    public async Task<AccountStatement> GetCostCenterStatement(DateTime? from, DateTime? to, int? costCenterId, bool openingBalance)
    {
        var costCenter = await _uow.CostCenter.Get(costCenterId.GetValueOrDefault());
        if (costCenter == null)
            return GetEmptyAccountStatement();

        var journals = (await _uow.JournalDetail
                   .GetAll(d =>
                     (d.CostCenterId == costCenterId)
                     && (!from.HasValue || d.Journal.CreatedAt.Date >= from.Value.Date)
                     && (!to.HasValue || d.Journal.CreatedAt.Date <= to.Value.Date)
                   , "CostCenter", "Journal"))
                   .DistinctBy(d => d.JournalId)
                   .OrderBy(d => d.Journal.CreatedAt);

        var journalsLinkedList = new LinkedList<AccountStatementDetail>();

        decimal balance = 0;
        if (openingBalance && from.HasValue)
        {
            var openingJournals = await _uow.JournalDetail
                    .GetAll(d => d.CostCenterId == costCenterId && d.Journal.CreatedAt < from.Value.Date, "Journal");

            if (openingJournals.Count > 0)
            {
                balance = Math.Abs(openingJournals.Sum(j => j.Debit));

                journalsLinkedList.AddFirst(new AccountStatementDetail
                {
                    Balance = balance,
                    Credit = openingJournals.Sum(j => j.Credit),
                    Debit = openingJournals.Sum(j => j.Debit),
                    Detail = "Opening Balance",
                    notes = "Opening Balance",
                    Date = openingJournals.First().Journal.CreatedAt.ToShortDateString(),
                });
            }
        }

        foreach (var journal in journals)
        {
            balance += journal.Debit;

            journalsLinkedList.AddLast(new AccountStatementDetail
            {
                Balance = balance,
                Credit = journal.Credit,
                Debit = journal.Debit,
                CostCenter = journal.CostCenter?.Name ?? "",
                Detail = journal.Journal.Detail,
                JournalId = journal.JournalId,
                notes = journal.Journal.Notes,
                PeriodId = journal.Journal.PeriodId,
                Date = journal.Journal.CreatedAt.ToShortDateString(),
            });
        }

        if (journalsLinkedList.Count > 0)
        {
            journalsLinkedList.AddLast(new AccountStatementDetail
            {
                Balance = balance,
                Credit = journalsLinkedList.Sum(j => j.Credit),
                Debit = journalsLinkedList.Sum(j => j.Debit),
                CostCenter = "",
                Detail = "Total",
                JournalId = 0,
                notes = "Total",
                PeriodId = 0,
                Date = "",
            });
        }
        return new AccountStatement()
        {
            AccountType = "",
            Details = journalsLinkedList,
            AccountName = costCenter.Name + " [ Cost Center ]",
            From = from?.ToString("d") ?? "",
            To = to?.ToString("d") ?? "",
            Amount = Math.Abs(balance).ToString("c")
        };
    }

    public async Task<IEnumerable<AccountSummaryDTO>> GetIncomeStatement(DateTime from, DateTime to)
    {
        var settings = await _uow.Settings.GetFirst();

        if (settings is null)
            return Enumerable.Empty<AccountSummaryDTO>();

        var expensesAccount = await _uow.Accounts.Get(settings.ExpensesAccount.GetValueOrDefault());
        var RevenuesAccount = await _uow.Accounts.Get(settings.RevenueAccount.GetValueOrDefault());

        if (RevenuesAccount is null || expensesAccount is null)
            return Enumerable.Empty<AccountSummaryDTO>();


        var journals = await _uow.JournalDetail
            .SelectAll(d => d.Journal.CreatedAt.Date >= from && d.Journal.CreatedAt.Date <= to
               && (d.Account.Number.StartsWith(expensesAccount.Number) || d.Account.Number.StartsWith(RevenuesAccount.Number))
            , d => new { accountId = d.AccountId, AccountNumber = d.Account.Number, AccountName = d.Account.Name, balance = d.Debit - d.Credit , d.Credit , d.Debit}
            , "Account", "Journal");



        var incomeStatement = journals.GroupBy(j => j.accountId)
                .Select(d => new AccountSummaryDTO
                {
                    AccountName = d.First().AccountName,
                    Balance = d.Sum(d => d.balance),
                    AccountNumber = d.First().AccountNumber,
                    Credit = d.Sum(d => d.Credit),
                    Debit = d.Sum(d => d.Debit),
                }).OrderByDescending(d => d.Balance).ToList();
                


        var totalExpenses = journals.Where(a => a.AccountNumber.StartsWith(expensesAccount.Number)).Sum(a => a.balance);
        var totalRevenues= journals.Where(a => a.AccountNumber.StartsWith(RevenuesAccount.Number)).Sum(a => a.balance * -1);

        var profit = totalRevenues - totalExpenses;

        if (profit > 0)
        {
            incomeStatement.Add(new AccountSummaryDTO
            {
                AccountNumber = "",
                AccountName = "Excess of Revenues Over Expenses",
                Credit = journals.Sum(a => a.Credit),
                Debit = journals.Sum(a => a.Debit),
                Balance = profit,
            });

        }
        else
        {
            incomeStatement.Add(new AccountSummaryDTO
            {
                AccountNumber = "",
                AccountName = "Excess of Expenses Over Revenues",
                Credit = journals.Sum(a => a.Credit),
                Debit = journals.Sum(a => a.Debit),
                Balance = profit * -1,

            });
        }

        return incomeStatement;
        }

    public async Task<IEnumerable<AccountSummaryDTO>> GetAccountsSummary(DateTime from, DateTime to)
    {

        var journals = await _uow.JournalDetail
            .SelectAll(d => d.Journal.CreatedAt.Date >= from && d.Journal.CreatedAt.Date <= to
            , d => new { accountId = d.AccountId, AccountNumber = d.Account.Number, AccountName = d.Account.Name, balance = d.Debit - d.Credit, d.Credit, d.Debit }
            , "Account", "Journal");

        if (journals.Count() == 0)
            return Enumerable.Empty<AccountSummaryDTO>();

        var incomeStatement = journals.GroupBy(j => j.accountId)
                .Select(d => new AccountSummaryDTO { 
                    AccountName = d.First().AccountName, 
                    AccountNumber = d.First().AccountNumber,
                    Credit = d.Sum(d => d.Credit),
                    Debit = d.Sum(d => d.Debit),
                    Balance = d.Sum(d => d.balance),
                
                }).OrderByDescending(a => a.Balance).ToList();

        incomeStatement.Add(new AccountSummaryDTO
        {
            AccountName = "Total",
            AccountNumber = "",
            Balance = journals.Sum(j => j.balance),
            Credit = journals.Sum(j => j.Credit),
            Debit = journals.Sum(j => j.Debit),
        });

        return incomeStatement;
    }

    public async Task<IEnumerable<AccountSummaryDTO>> GetAccountsOverview(DateTime from, DateTime to, int? maxLevel)
    {
        var currentJournalAccounts = (await _uow.JournalDetail
            .SelectAll(d => d.Journal.CreatedAt.Date >= from && d.Journal.CreatedAt.Date <= to
            , d => new { accountId = d.AccountId, AccountNumber = d.Account.Number, AccountName = d.Account.Name, balance = d.Debit - d.Credit, d.Credit, d.Debit }
            , "Account", "Journal"))
            .GroupBy(x => x.AccountNumber)
            .Select(j => new {AccountNumber = j.Key , Debit = j.Sum(a => a.Debit) , Credit = j.Sum(a => a.Credit)});

        if (!currentJournalAccounts.Any())
            return Enumerable.Empty<AccountSummaryDTO>();

        var accounts = await _uow.Accounts.GetAll(d => !maxLevel.HasValue || d.Level <= maxLevel.Value);

        var accountsSummaries = new LinkedList<AccountSummaryDTO>();

        foreach (var account in accounts)
        {
            var debit = currentJournalAccounts.Where(j => j.AccountNumber.StartsWith(account.Number)).Sum(d => d.Debit);
            var credit = currentJournalAccounts.Where(j => j.AccountNumber.StartsWith(account.Number)).Sum(d => d.Credit);

            var balance = debit - credit;

            if (balance != 0)
            { 
                accountsSummaries.AddLast(
                    new AccountSummaryDTO()
                    {
                        AccountName = string.Concat(new string(' ',(account.Level - 1) * 5) , account.Name),
                        AccountNumber = account.Number,
                        AccountId = account.Id,
                        Debit = debit,
                        Credit = credit,
                        Balance = balance
                    });
            }
        }

        return  accountsSummaries.OrderBy(s => s.AccountNumber);
    }

    public async Task<AccountComparerDTO> GetAccountComparer(DateTime? from, DateTime? to, int accountId, int? costCenterId, AccountComparerGroups groupType)
    {
        // Get Accounts 
        var account = await _uow.Accounts.Get(accountId);
        if (account is null)
            return new AccountComparerDTO();

        

        var journals = (await _uow.JournalDetail
        .GetAll(d =>
             d.Account.Number.StartsWith(account.Number)
            && (!from.HasValue || d.Journal.CreatedAt.Date >= from.Value.Date)
            && (!to.HasValue || d.Journal.CreatedAt.Date <= to.Value.Date)
            && (!costCenterId.HasValue || d.CostCenterId == costCenterId)
            , "CostCenter", "Account", "Journal")).OrderBy(j => j.Journal.CreatedAt);



        IEnumerable<AccountComparerItemDTO> journalGroupKey = Enumerable.Empty<AccountComparerItemDTO>();

        switch (groupType)
        {
            case AccountComparerGroups.ByDay:
                journalGroupKey = journals.GroupBy(j => j.Journal.CreatedAt.Date)
                    .Select(k => new AccountComparerItemDTO
                    {
                        Amount = k.Sum(a => a.Debit - a.Credit),
                        Time = k.Key.ToShortDateString()
                    });
                break;

            case AccountComparerGroups.ByMonth:
                journalGroupKey = journals.GroupBy(j => j.Journal.CreatedAt.Month)
                    .Select(k => new AccountComparerItemDTO
                    {
                        Amount = k.Sum(a => a.Debit - a.Credit),
                        Time = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(k.Key),
                    });
                break;

            case AccountComparerGroups.ByYear:
                journalGroupKey = journals.GroupBy(j => j.Journal.CreatedAt.Year)
                    .Select(k => new AccountComparerItemDTO
                    {
                        Amount = k.Sum(a => a.Debit - a.Credit),
                        Time = k.Key.ToString(),
                    });
                break;

            default:
                journalGroupKey = journals.GroupBy(j => j.Journal.CreatedAt.Month)
                    .Select(k => new AccountComparerItemDTO
                    {
                        Amount = k.Sum(a => a.Debit - a.Credit),
                        Time = k.Key.ToString(),
                    });
                break;
        }

        return new AccountComparerDTO()
        {
            GroupType = groupType.ToString(), 
            From = from.GetValueOrDefault().ToShortDateString(),
            To = to.GetValueOrDefault().ToShortDateString(),
            Label = account.Name,
            Amounts = journalGroupKey.Select(j => j.Amount),
            Time = journalGroupKey.Select(j => j.Time),
        };        
    }
    private AccountStatement GetEmptyAccountStatement()
    {
        return new AccountStatement()
        {
            AccountType = "",
            AccountName = "Not Matched Any Results",
            Amount = 0.ToString("c"),
            Details = Enumerable.Empty<AccountStatementDetail>()
        };
    }
}
