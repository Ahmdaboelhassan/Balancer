using Domain.DTO.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.IServices;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;

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

        var journals = await _uow.JournalDetail
                   .SelectAllOrderBy(d =>
                        d.Account.Number.StartsWith(account.Number)
                     && (!from.HasValue || d.Journal.CreatedAt.Date >= from.Value.Date)
                     && (!to.HasValue || d.Journal.CreatedAt.Date <= to.Value.Date)
                     && (!costCenterId.HasValue || d.CostCenters.Any(d => d.CostCenterId == costCenterId)),
                     j => new
                     {
                         Journal = j.Journal,
                         JournalDetail = j,
                         CreditAccount = j.Journal.JournalDetails.Where(d => d.Credit > 0).Select(d => d.Account.Name).FirstOrDefault(),
                         DebitAccount = j.Journal.JournalDetails.Where(d => d.Debit > 0).Select(d => d.Account.Name).FirstOrDefault(),
                         CostCenters = j.CostCenters.Select(d => d.CostCenter.Name)
                     }, d => d.Journal.CreatedAt
                   );


        var journalsLinkedList = new List<AccountStatementDetail>(journals.Count());

        decimal balance = 0;

        if (openingBalance && from.HasValue)
        {
            var openingJournal = await _uow.JournalDetail.AsQueryable()
                .Where(d =>
                    d.Account.Number.StartsWith(account.Number)
                    && d.Journal.CreatedAt < from.Value.Date
                    && (!costCenterId.HasValue ||
                        d.CostCenters.Any(cc => cc.CostCenterId == costCenterId)))
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Debit = g.Sum(x => x.Debit),
                    Credit = g.Sum(x => x.Credit),
                    Balance = g.Sum(x => x.Debit - x.Credit),
                    Date = g.Min(x => x.Journal.CreatedAt)
                })
                .FirstOrDefaultAsync();

            if (openingJournal != null)
            {
                balance += openingJournal.Balance;

                journalsLinkedList.Add(new AccountStatementDetail
                {
                    Balance = balance,
                    Credit = openingJournal.Credit,
                    Debit = openingJournal.Debit,
                    Detail = "Opening Balance",
                    notes = "Opening Balance",
                    Description = "Opening Balance",
                    Date = openingJournal.Date.ToShortDateString(),
                });
            }
        }

        foreach (var journal in journals)
        {
            balance += journal.JournalDetail.Debit - journal.JournalDetail.Credit;

            journalsLinkedList.Add(new AccountStatementDetail
            {
                Balance = balance,
                Credit = journal.JournalDetail.Credit,
                Debit = journal.JournalDetail.Debit,
                CostCenters = journal.CostCenters,
                Detail = journal.Journal.Detail,
                JournalId = journal.Journal.Id,
                notes = $"{journal.CreditAccount} -> {journal.DebitAccount}",
                PeriodId = journal.Journal.PeriodId,
                Description = journal.Journal.Description,
                Date = journal.Journal.CreatedAt.ToShortDateString(),
            });
        }

        if (journalsLinkedList.Count > 0)
        {
            journalsLinkedList.Add(new AccountStatementDetail
            {
                Balance = balance,
                Credit = journalsLinkedList.Sum(j => j.Credit),
                Debit = journalsLinkedList.Sum(j => j.Debit),
                Detail = "Total",
                JournalId = 0,
                notes = "Total",
                Description = "",
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
        var settings = await _uow.Settings.GetFirst();
        var costCenter = await _uow.CostCenter.Get(costCenterId.GetValueOrDefault());

        if (costCenter is null || settings is null || !settings.CurrentCashAccount.HasValue)
            return GetEmptyAccountStatement();

        var currentCashAccount = await _uow.Accounts.Get(settings.CurrentCashAccount.GetValueOrDefault());

        if (currentCashAccount is null)
            return GetEmptyAccountStatement();

        var journals = await _uow.JournalDetail
                   .SelectAllOrderBy(d =>
                     (d.CostCenters.Any(cc => cc.CostCenterId == costCenterId) && !d.Account.Number.StartsWith(currentCashAccount.Number))
                     && (!from.HasValue || d.Journal.CreatedAt.Date >= from.Value.Date)
                     && (!to.HasValue || d.Journal.CreatedAt.Date <= to.Value.Date),
                        j => new
                        {
                            Journal = j.Journal,
                            JournalDetail = j,
                            CreditAccount = j.Journal.JournalDetails.Where(d => d.Credit > 0).Select(d => d.Account.Name).FirstOrDefault(),
                            DebitAccount = j.Journal.JournalDetails.Where(d => d.Debit > 0).Select(d => d.Account.Name).FirstOrDefault(),
                            CostCenters = j.CostCenters.Select(d => d.CostCenter.Name)
                        },
                        d => d.Journal.CreatedAt
                   );

        var journalsList = new List<AccountStatementDetail>(journals.Count());

        decimal balance = 0;
        if (openingBalance && from.HasValue)
        {
            var openingJournals = await _uow.JournalDetail.AsQueryable()
                .Where(d => d.CostCenters.Any(cc => cc.CostCenterId == costCenterId)
                    && !d.Account.Number.StartsWith(currentCashAccount.Number)
                    && d.Journal.CreatedAt < from.Value.Date)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Debit = g.Sum(x => x.Debit),
                    Credit = g.Sum(x => x.Credit),
                    Balance = g.Sum(x => x.Debit - x.Credit),
                    Date = g.Min(x => x.Journal.CreatedAt)
                })
                .FirstOrDefaultAsync();

            if (openingJournals != null)
            {
                balance += openingJournals.Balance;

                journalsList.Add(new AccountStatementDetail
                {
                    Balance = balance,
                    Credit = openingJournals.Credit,
                    Debit = openingJournals.Debit,
                    Detail = "Opening Balance",
                    notes = "Opening Balance",
                    Date = openingJournals.Date.ToShortDateString(),
                    CostCenters = [costCenter.Name]
                });
            }
        }

        foreach (var journal in journals)
        {
            balance += journal.JournalDetail.Debit - journal.JournalDetail.Credit;

            journalsList.Add(new AccountStatementDetail
            {
                Balance = balance,
                Credit = journal.JournalDetail.Credit,
                Debit = journal.JournalDetail.Debit,
                CostCenters = journal.CostCenters,
                Detail = journal.Journal.Detail,
                JournalId = journal.Journal.Id,
                notes = $"{journal.CreditAccount} -> {journal.DebitAccount}",
                PeriodId = journal.Journal.PeriodId,
                Description = journal.Journal.Description,
                Date = journal.Journal.CreatedAt.ToShortDateString(),
            });
        }

        if (journalsList.Count > 0)
        {
            journalsList.Add(new AccountStatementDetail
            {
                Balance = balance,
                Credit = journalsList.Sum(j => j.Credit),
                Debit = journalsList.Sum(j => j.Debit),
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
            Details = journalsList,
            AccountName = costCenter.Name + " (Cost Center)",
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
            , d => new { accountId = d.AccountId, AccountNumber = d.Account.Number, AccountName = d.Account.Name, balance = d.Debit - d.Credit , d.Credit , d.Debit});



        var incomeStatement = journals.GroupBy(j => j.accountId)
                .Select(d => new AccountSummaryDTO
                {
                    AccountId = d.First().accountId,
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

    public async Task<BudgetSummaryReportDTO> GetBudgetSummary(DateTime from, DateTime to)
    {
        var settings = await _uow.Settings.GetFirst();
        var dashboardSettings = await _uow.DashboardSettings.GetFirst();

        if (settings is null || dashboardSettings is null ||
            !settings.ExpensesAccount.HasValue ||
            !settings.FixedAssetsAccount.HasValue)
            return new BudgetSummaryReportDTO();

        // PERIODS BUDGET SUMMARY
        var periods = await _uow.Periods.GetAll(p =>
            (p.From >= from.Date || p.To >= from.Date) &&
            p.From <= to.Date);

        var periodSummaries = periods.Select(period =>
        {
            var remaining = period.PeriodBudget.HasValue && period.PeriodBudget > 0 ? period.TotalAmount - period.PeriodBudget : (period.PeriodBudget.HasValue ? Math.Abs(period.PeriodBudget.Value) - Math.Abs(period.TotalAmount) : null);

            return new BudgetSummaryDTO
            {
                Id = period.Id,
                Name = period.Name,
                Spent = period.TotalAmount,
                Budget = period.PeriodBudget,
                Remains = remaining,
                RemainPercentage = remaining.HasValue && period.PeriodBudget.HasValue ? (remaining.Value / Math.Abs(period.PeriodBudget.Value)) * 100 : null,
                SpentPercentage = period.PeriodBudget.HasValue && period.PeriodBudget != 0 ? (period.TotalAmount / period.PeriodBudget.Value) * 100 : null,
            };
        }).ToList();

        var totalPeriodSpent = periodSummaries.Sum(x => x.Spent);
        var totalPeriodRemaining = periodSummaries.Sum(x => x.Remains ?? 0);
        var totalPeriodBudget = periodSummaries.Sum(x => x.Budget ?? 0);

        periodSummaries.Add(new BudgetSummaryDTO
        {
            Id = 0,
            Name = "",
            Spent = totalPeriodSpent,
            Budget = totalPeriodBudget,
            Remains = totalPeriodRemaining,
            RemainPercentage = totalPeriodBudget > 0 ? (totalPeriodRemaining / totalPeriodBudget) * 100 : null,
            SpentPercentage = totalPeriodBudget > 0 ? (totalPeriodSpent / totalPeriodBudget) * 100 : null,
        });

       
        // JOURNAL DATA
       
        var journals = await _uow.JournalDetail.GetAll(
            j => j.Journal.CreatedAt.Date >= from.Date &&
                 j.Journal.CreatedAt.Date <= to.Date,
            "Journal",
            "Account",
            "CostCenters");

        var journalTotalsByAccount = journals
            .GroupBy(j => j.Account.Number)
            .Select(g => new
            {
                AccountNumber = g.Key,
                Total = g.Sum(x => x.Debit - x.Credit)
            });

        var budgetAccounts = await _uow.BudgetAccounts.GetAll("Account");

        var expenseAccounts = (await _uow.Accounts.GetAll(a => a.Id == settings.ExpensesAccount || a.Id == settings.FixedAssetsAccount)) .Select(a => a.Number).ToList();

        decimal overBudgetTotal = 0;

        bool IsExpenseAccount(string accountNumber) => expenseAccounts.Any(a => accountNumber.StartsWith(a));

        // BUDGET PROGRESS
        var budgetProgress = budgetAccounts
            .GroupJoin(
                journalTotalsByAccount,
                budget => budget.Account.Number,
                journal => journal.AccountNumber,
                (budget, journalGroup) => new { budget, journalGroup })
            .SelectMany(
                x => x.journalGroup.DefaultIfEmpty(),
                (x, journal) =>
                {
                    var spent = journalTotalsByAccount
                        .Where(d => d.AccountNumber.StartsWith(x.budget.Account.Number))
                        .Sum(d => d.Total);

                    if (IsExpenseAccount(x.budget.Account.Number))
                        overBudgetTotal += Math.Max(0, spent - x.budget.Budget);

                    var remains = x.budget.Budget - spent;
                    return new BudgetSummaryDTO
                    {
                        Id = x.budget.AccountId,
                        Name = x.budget.DisplayName,
                        Budget = x.budget.Budget,
                        Spent = spent,
                        Remains = remains,
                        SpentPercentage = x.budget.Budget != 0 ? Math.Round((spent / x.budget.Budget) * 100) : 0,
                        RemainPercentage = x.budget.Budget != 0 ? Math.Round((remains / x.budget.Budget) * 100) : 0,
                    };
                })
            .ToList();

        var excludedAccountNumbers = budgetAccounts
            .Where(a => IsExpenseAccount(a.Account.Number))
            .Select(a => a.Account.Number);

        var otherExpenses = journals
            .Where(x => IsExpenseAccount(x.Account.Number) && (settings.NotBudgetCostCenter.HasValue 
            ? x.CostCenters.Any(x => x.CostCenterId == settings.NotBudgetCostCenter) 
            : !excludedAccountNumbers.Any(a => x.Account.Number.StartsWith(a))))
            .Sum(x => x.Debit - x.Credit);

        if (dashboardSettings.ApplyOverBudgetToFunds)
            otherExpenses += overBudgetTotal;

        var otherExpensesTarget =  dashboardSettings.OtherExpensesTarget + dashboardSettings.AddOnExpensesTarget;

        budgetProgress = budgetProgress.OrderByDescending(x => x.Remains).ToList();

        // TOTAL ACCOUNTS SUMMARY

        budgetProgress.Add(new BudgetSummaryDTO
        {
            Id = settings.NotBudgetCostCenter ?? 0,
            IsOtherExpenses = true,
            Name = "Other Expenses",
            Budget = otherExpensesTarget,
            Spent = otherExpenses,
            SpentPercentage = otherExpensesTarget != 0 ? Math.Round((otherExpenses / otherExpensesTarget) * 100) : 0,
            Remains = otherExpensesTarget - otherExpenses,
            RemainPercentage = otherExpensesTarget != 0 ? Math.Round(((otherExpensesTarget - otherExpenses) / dashboardSettings.OtherExpensesTarget) * 100) : 0,
        });



        var expensesAccounts = budgetProgress.Where(d => d.Id != settings.CurrentCashAccount).ToList();

        var totalAccountsSpent = expensesAccounts.Sum(x => x.Spent);
        var totalAccountsRemaining = expensesAccounts.Sum(x => x.Remains ?? 0);
        var totalAccountsBudget = expensesAccounts.Sum(x => x.Budget ?? 0);

        expensesAccounts.Add(new BudgetSummaryDTO
        {
            Id = 0,
            Name = "",
            Spent = totalAccountsSpent,
            Budget = totalAccountsBudget,
            Remains = totalAccountsRemaining,
            RemainPercentage = totalAccountsBudget > 0
                ? (totalAccountsRemaining / totalAccountsBudget) * 100
                : null,
            SpentPercentage = totalAccountsBudget > 0
                ? (totalAccountsSpent / totalAccountsBudget) * 100
                : null,
        });


        var savings = budgetProgress
            .Where(d => d.Id == settings.CurrentCashAccount || d.Id == settings.CurrentAssetsAccount)
            .ToList();

        return new BudgetSummaryReportDTO
        {
            Periods = periodSummaries,
            Accounts = expensesAccounts,
            Savings = savings
        };
    }
    public async Task<IEnumerable<AccountSummaryDTO>> GetAccountsSummary(DateTime from, DateTime to)
    {

        var journals = await _uow.JournalDetail
            .SelectAll(d => d.Journal.CreatedAt.Date >= from && d.Journal.CreatedAt.Date <= to
            , d => new { accountId = d.AccountId, AccountNumber = d.Account.Number, AccountName = d.Account.Name, balance = d.Debit - d.Credit, d.Credit, d.Debit });

        if (journals.Count() == 0)
            return Enumerable.Empty<AccountSummaryDTO>();

        var incomeStatement = journals.GroupBy(j => j.accountId)
                .Select(d => new AccountSummaryDTO { 
                    AccountId = d.First().accountId,
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
            , d => new { accountId = d.AccountId, AccountNumber = d.Account.Number, AccountName = d.Account.Name, balance = d.Debit - d.Credit, d.Credit, d.Debit }))
            .GroupBy(x => x.AccountNumber)
            .Select(j => new {AccountNumber = j.Key , Debit = j.Sum(a => a.Debit) , Credit = j.Sum(a => a.Credit)});

        if (!currentJournalAccounts.Any())
            return Enumerable.Empty<AccountSummaryDTO>();

        
        List<Account> accounts;

        if (maxLevel.HasValue)
            if (maxLevel.Value > 10 )
                accounts = await _uow.Accounts.GetAll(d => d.Level == (maxLevel.Value - 10));
            else if (maxLevel.Value > 5)
                accounts = await _uow.Accounts.GetAll(d => d.Level >= (maxLevel.Value - 5));
            else 
                accounts = await _uow.Accounts.GetAll(d => d.Level <= maxLevel.Value);
        else
            accounts = await _uow.Accounts.GetAll();


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
    public async Task<IEnumerable<AccountSummaryDTO>> GetBalanceSheet(DateTime to, int? maxLevel)
    {

        var settings = await _uow.Settings.GetFirst();
        if (settings is null || settings.AssetsAccount is null || settings.LiabilitiesAccount is null)
            return Enumerable.Empty<AccountSummaryDTO>();

        int assetsAccountId = settings.AssetsAccount.Value;
        int LiabilitiesAccountId = settings.LiabilitiesAccount.Value;

        var accounts = await _uow.Accounts.GetAll(x => x.Id == assetsAccountId || x.Id == LiabilitiesAccountId);
        var accountNumbers = accounts.ToDictionary(x => x.Id, x => x.Number);

        var currentJournalAccounts = (await _uow.JournalDetail
            .SelectAll(d => d.Journal.CreatedAt.Date <= to
            && (d.Account.Number.StartsWith(accountNumbers[assetsAccountId]) || d.Account.Number.StartsWith(accountNumbers[LiabilitiesAccountId]))
            , d => new { accountId = d.AccountId, AccountNumber = d.Account.Number, AccountName = d.Account.Name, balance = d.Debit - d.Credit, d.Credit, d.Debit }));

        if (!currentJournalAccounts.Any())
            return Enumerable.Empty<AccountSummaryDTO>();

        Expression<Func<Account, bool>> filter = 
            d => (d.Number.StartsWith(accountNumbers[assetsAccountId]) || d.Number.StartsWith(accountNumbers[LiabilitiesAccountId]));

        
        if (maxLevel.HasValue)
            if (maxLevel.Value > 10)
                    filter = d => d.Level == (maxLevel.Value - 10) && (d.Number.StartsWith(accountNumbers[assetsAccountId]) || d.Number.StartsWith(accountNumbers[LiabilitiesAccountId]));
            else if (maxLevel.Value > 5)
                filter = d => d.Level >= (maxLevel.Value - 5) && (d.Number.StartsWith(accountNumbers[assetsAccountId]) || d.Number.StartsWith(accountNumbers[LiabilitiesAccountId]));
            else
                filter = d => (!maxLevel.HasValue || d.Level <= maxLevel.Value) && (d.Number.StartsWith(accountNumbers[assetsAccountId]) || d.Number.StartsWith(accountNumbers[LiabilitiesAccountId]));

        var childAccountNumber = await _uow.Accounts.GetAll(filter);
          

        var accountsSummaries = new LinkedList<AccountSummaryDTO>();

        foreach (var account in childAccountNumber)
        {
            var debit = currentJournalAccounts.Where(j => j.AccountNumber.StartsWith(account.Number)).Sum(d => d.Debit);
            var credit = currentJournalAccounts.Where(j => j.AccountNumber.StartsWith(account.Number)).Sum(d => d.Credit);

            var balance = debit - credit;

            if (balance != 0)
            {
                accountsSummaries.AddLast(
                    new AccountSummaryDTO()
                    {
                        AccountName = string.Concat(new string(' ', (account.Level - 1) * 5), account.Name),
                        AccountNumber = account.Number,
                        AccountId = account.Id,
                        Debit = balance > 0 ? balance : 0,
                        Credit = balance < 0 ? Math.Abs(balance) : 0,
                    });
            }
        }

        return accountsSummaries.OrderBy(s => s.AccountNumber);
    }

    public async Task<AccountComparerDTO> GetAccountComparer(DateTime? from, DateTime? to, int accountId, int? costCenterId, AccountComparerGroups groupType, bool openingBalance)
    {
        // Get Accounts 
        var account = await _uow.Accounts.Get(accountId);
        if (account is null || !from.HasValue || !to.HasValue)
            return new AccountComparerDTO();

        var query = _uow.JournalDetail.AsQueryable()
            .Where(d =>
                d.Account.Number.StartsWith(account.Number)
                && (d.Journal.CreatedAt.Date >= from.Value.Date)
                && d.Journal.CreatedAt.Date <= to.Value.Date
                && (!costCenterId.HasValue || d.CostCenters.Any(cc => cc.CostCenterId == costCenterId)));


       
    

        IEnumerable<AccountComparerItemDTO> journalGroupKey = Enumerable.Empty<AccountComparerItemDTO>();
        
        switch (groupType)
        {
            case AccountComparerGroups.ByDay:
                journalGroupKey = await query
                    .GroupBy(j => j.Journal.CreatedAt.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new AccountComparerItemDTO
                    {
                        Amount = g.Sum(x => x.Debit - x.Credit),
                        Time = g.Key.ToShortDateString()
                    })
                    .ToListAsync();

                break;

            case AccountComparerGroups.ByMonth:
                journalGroupKey = (await query
                    .GroupBy(j => new { j.Journal.CreatedAt.Year, j.Journal.CreatedAt.Month })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        Amount = g.Sum(x => x.Debit - x.Credit)
                    })
                    .ToListAsync())
                    .Select(x => new AccountComparerItemDTO
                    {
                        Amount = x.Amount,
                        Time = new DateTime(x.Year, x.Month, 1).ToString("MMMM yy", CultureInfo.CurrentCulture)
                    })
                    .ToList();

                break;

            case AccountComparerGroups.ByYear:
                journalGroupKey = await query
                    .GroupBy(j => j.Journal.CreatedAt.Year)
                    .OrderBy(g => g.Key)
                    .Select(g => new AccountComparerItemDTO
                    {
                        Amount = g.Sum(x => x.Debit - x.Credit),
                        Time = g.Key.ToString()
                    })
                    .ToListAsync();

                break;

            default:
                journalGroupKey = Enumerable.Empty<AccountComparerItemDTO>();
                break;
        }


        // Add Running Balance 
        if (openingBalance)
        {
            var runningBalance = await _uow.JournalDetail.AsQueryable()
                  .Where(d =>
                      d.Account.Number.StartsWith(account.Number)
                      && d.Journal.CreatedAt.Date < from.Value.Date
                      && (!costCenterId.HasValue || d.CostCenters.Any(cc => cc.CostCenterId == costCenterId)))
                  .SumAsync(x => x.Debit - x.Credit);

            foreach (var item in journalGroupKey)
            {
                runningBalance += item.Amount;
                item.Amount = runningBalance;
            }
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
