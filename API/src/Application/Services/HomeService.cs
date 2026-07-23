using Domain.DTO.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.IServices;
using Domain.Static;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Application.Services;

public class HomeService : IHomeService
{
    private readonly IAccountService _accountService;
    private readonly IServiceScopeFactory _scope;

    public HomeService(IAccountService accountService, IServiceScopeFactory scope)
    {
        _accountService = accountService;
        _scope = scope;
    }

    public async Task<GetHomeDTO> GetHome()
    {
        // Statistics
        await using var scope = _scope.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var dashboard = await unitOfWork.DashboardSettings.GetFirst();
        var settings = await unitOfWork.Settings.GetFirst();
        if (dashboard == null || settings == null)
            return GetEmptyHome();

        var accountIds = new[]
        {
            dashboard.Account1,
            dashboard.Account2,
            dashboard.Account3,
            dashboard.Account4,
            settings.ExpensesAccount.GetValueOrDefault(),
            settings.FixedAssetsAccount.GetValueOrDefault(),
        };

        var accounts = (await unitOfWork.Accounts.GetAll(x => accountIds.Contains(x.Id))).ToDictionary(a => a.Id, a => a);

        if (accounts.Count() < 4)
            return GetEmptyHome();

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(MagicStrings.TimeZone);
        var current = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

        var otherExpensesAccounts = new HashSet<string>(
        [
            accounts[accountIds[4]].Number,
            accounts[accountIds[5]].Number
        ]);

        var accountsBalancesTasks = new[]
        {
            GetDashboardAccountBalance(accounts[accountIds[0]]),
            GetDashboardAccountBalance(accounts[accountIds[1]]),
            GetDashboardAccountBalance(accounts[accountIds[2]]),
            GetDashboardAccountBalance(accounts[accountIds[3]]),
        };

        var balances = await Task.WhenAll(accountsBalancesTasks);


        // Line Chart
        var lastPeriodsTask = GetLastPeriodsBalancesAsync(4);

        // Pie Chart
        var journalsTypesSummaryTask = GetJournalsTypesSummary(current);

        // Bar Chart
        var lastMonthsJournalsTask = GetLast12MonthJournalAsync(current);

        // Progress Bar 
        var GetBudgetProgressTask = GetBudgetProgressAsync(dashboard, settings, otherExpensesAccounts, current);

        await Task.WhenAll(lastPeriodsTask, journalsTypesSummaryTask, lastMonthsJournalsTask, GetBudgetProgressTask);

        return new GetHomeDTO
        {
            AccountsSummary = balances,
            LastPeriods = lastPeriodsTask.Result,
            Expenses = lastMonthsJournalsTask.Result.expenses,
            Revenues = lastMonthsJournalsTask.Result.revenues,
            MonthsNames = lastMonthsJournalsTask.Result.months,
            JournalsTypesSummary = journalsTypesSummaryTask.Result,
            AvailableFunds = GetBudgetProgressTask.Result.availableFunds,
            OtherExpenses = GetBudgetProgressTask.Result.otherExpenses,
            OtherExpensesTarget = dashboard.OtherExpensesTarget + dashboard.AddOnExpensesTarget,
            BudgetProgress = GetBudgetProgressTask.Result.budgetProgress.Where(d => d.Spent < d.Budget || d.AccountId == settings.CurrentCashAccount),
            DayRate = settings.DefaultDayRate,
            PeriodDays = settings.DefaultPeriodDays.GetValueOrDefault(),
        };
    }

    private async Task<(IEnumerable<BudgetAccountDTO> budgetProgress, decimal otherExpenses, decimal availableFunds)> 
        GetBudgetProgressAsync(DashboardSettings dashboard, Settings settings, HashSet<string> accountSet, DateTime current)
    {
        await using var scope = _scope.CreateAsyncScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var currentMonthJournals = (await unitOfWork.JournalDetail
                   .GetAll(j => (j.Journal.CreatedAt.Month == current.Month && j.Journal.CreatedAt.Year == current.Year), "Journal", "Account", "CostCenters"));


        var groupedJournals = currentMonthJournals 
                   .GroupBy(j => j.Account.Number)
                   .Select(j => new { AccountNumber = j.Key, Total = j.Sum(j => j.Debit - j.Credit) });

        var budgetAccounts = await unitOfWork.BudgetAccounts.GetAll("Account");

        decimal overBudget = 0;

        var IsAccountNumberInAccountSet = (string n) => accountSet.Any(a => n.StartsWith(a));

        var budgetProgress = budgetAccounts
             .GroupJoin(groupedJournals,
                 ba => ba.Account.Number,
                 cmj => cmj.AccountNumber,
                 (ba, cmjGroup) => new { ba, cmjGroup })
             .SelectMany(
                 x => x.cmjGroup.DefaultIfEmpty(),
                 (x, cmj) =>
                 {
                     var totalSpent = groupedJournals
                         .Where(d => d.AccountNumber.StartsWith(x.ba.Account.Number))
                         .Sum(d => d.Total);

                     if (IsAccountNumberInAccountSet(x.ba.Account.Number))
                        overBudget += Math.Max(0, totalSpent - x.ba.Budget);

                     return new BudgetAccountDTO
                     {   AccountId =  x.ba.AccountId,
                         DisplayName = x.ba.DisplayName,
                         Budget = x.ba.Budget,
                         Spent = totalSpent,
                         Percentage = (x.ba.Budget != 0) ? Math.Round((totalSpent / x.ba.Budget) * 100) : 0,
                         Color = x.ba.Color
                     };
                 }).ToList();

        var excludedAccounts = budgetAccounts.Where(a => IsAccountNumberInAccountSet(a.Account.Number)).Select(x => x.Account.Number);

        var otherExpenses = currentMonthJournals
            .Where(x => IsAccountNumberInAccountSet(x.Account.Number) 
            && (settings.NotBudgetCostCenter.HasValue 
            ? (x.CostCenters.Any(cc => cc.CostCenterId == settings.NotBudgetCostCenter))
            : !excludedAccounts.Any(a => x.Account.Number.StartsWith(a))))
            .Sum(x => x.Debit - x.Credit);


        if (dashboard.ApplyOverBudgetToFunds)
            otherExpenses += overBudget;

        var availableFunds = dashboard.OtherExpensesTarget - otherExpenses + dashboard.AddOnExpensesTarget ;
        
        return (budgetProgress, otherExpenses, availableFunds);
    }
    private async Task<IEnumerable<decimal>> GetLastPeriodsBalancesAsync(int count)
    {
        await using var scope = _scope.CreateAsyncScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        return (await unitOfWork.Periods.
                TakeLastOrderBy(count, p => new { p.From, p.TotalAmount }, p => p.From.Date))
                .OrderBy(p => p.From).Select(p => p.TotalAmount);
    }
    private async Task<IEnumerable<decimal>> GetJournalsTypesSummary(DateTime current)
    {
        await using var scope = _scope.CreateAsyncScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();


        // Pie Chart
        var journalsTypesSummary = (await unitOfWork.Journal
               .GetAll(j => j.CreatedAt.Date.Month == current.Month && j.CreatedAt.Date.Year == current.Year))
               .GroupBy(j => j.Type)
               .Select(j => new { j.Key, total = j.Sum(j => Math.Abs(j.Amount)) })
               .ToDictionary(x => x.Key, x => x.total);

        return new List<decimal> {
            journalsTypesSummary.GetValueOrDefault((byte)JournalTypes.Add),
            journalsTypesSummary.GetValueOrDefault((byte)JournalTypes.Subtract),
            journalsTypesSummary.GetValueOrDefault((byte)JournalTypes.Forward),
            journalsTypesSummary.GetValueOrDefault((byte)JournalTypes.Due),
            journalsTypesSummary.GetValueOrDefault((byte)JournalTypes.Investment),
        };

    }
    private async Task<(List<decimal> expenses, List<decimal> revenues, List<string> months)> GetLast12MonthJournalAsync(DateTime current)
    {
        await using var scope = _scope.CreateAsyncScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Bar Chart 
        var last11Month = current.AddMonths(-11).Date;
        var firstDayInLast11Month = new DateTime(last11Month.Year, last11Month.Month, 1);
        var lastDayOfCurrentMonth = current.AddMonths(1).AddDays(-1).Date;
        

        List<decimal> expenses = new List<decimal>();
        List<decimal> revenues = new List<decimal>();
        List<string> months = new List<string>();

        var currentYearJournal = (await unitOfWork.Journal
                .GetAll(j => j.CreatedAt.Date >= firstDayInLast11Month.Date && j.CreatedAt.Date <= lastDayOfCurrentMonth.Date && (j.Type == (byte)JournalTypes.Subtract || j.Type == (byte)JournalTypes.Add)))
                .OrderBy(j => j.CreatedAt);

        var expensesMonthlyGrouped = currentYearJournal.Where(j => j.Type == (byte)JournalTypes.Subtract)
                                                       .GroupBy(j => new { j.CreatedAt.Year, j.CreatedAt.Month })
                                                       .Select(j => new { j.Key, Total = j.Sum(j => j.Amount * -1) })
                                                       .ToDictionary(j => j.Key, j => j.Total);


        var revenuesMonthlyGrouped = currentYearJournal.Where(j => j.Type == (byte)JournalTypes.Add)
                                                        .GroupBy(j => new { j.CreatedAt.Year, j.CreatedAt.Month })
                                                       .Select(j => new { j.Key, Total = j.Sum(j => j.Amount) })
                                                       .ToDictionary(j => j.Key, j => j.Total);


        var keys = expensesMonthlyGrouped.Keys;

        foreach (var k in keys)
        {
            expenses.Add(expensesMonthlyGrouped.TryGetValue(k, out decimal expensesValue) ? expensesValue : 0);

            revenues.Add(revenuesMonthlyGrouped.TryGetValue(k, out decimal revenueValue) ? revenueValue : 0);

            var month = new DateTime(k.Year, k.Month, 1).ToString("MMM yy", CultureInfo.CurrentCulture);

            months.Add(month);
        }

        return (expenses, revenues, months);
    }
    private async Task<AccountBalanceDTO> GetDashboardAccountBalance(Account account)
    {
        await using var scope = _scope.CreateAsyncScope();

        var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();

        var balance = await accountService.GetBalance(account.Id);

        return new AccountBalanceDTO
        {
            AccountName = account.Name,
            Balance = balance.ToString("C")
        };
    }

    private GetHomeDTO GetEmptyHome()
    {
        return new GetHomeDTO
        {
            AccountsSummary = new List<AccountBalanceDTO>() {
                 new AccountBalanceDTO { AccountName = "No Account", Balance = "0" },
                 new AccountBalanceDTO { AccountName = "No Account", Balance = "0" },
                 new AccountBalanceDTO { AccountName = "No Account", Balance = "0" },
                 new AccountBalanceDTO { AccountName = "No Account", Balance = "0" },
            },
            LastPeriods = new List<decimal> { 0 , 0 , 0 , 0},
            JournalsTypesSummary = new List<decimal> { 0, 0},
            Expenses = new List<decimal> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            Revenues = new List<decimal> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            MonthsNames = new List<string> {
                "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
            },
            AvailableFunds = 0,
            DayRate = 0,
            PeriodDays = 0,
            BudgetProgress = new List<BudgetAccountDTO> { 
                new BudgetAccountDTO { 
                    Budget = 0,
                    Color = "",
                    DisplayName = "No Account",
                    Percentage = 0,
                    Spent = 0
                
                } }
           
        };
    }


}

