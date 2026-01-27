using Domain.DTO.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.IServices;
using Microsoft.AspNetCore.Localization;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Application.Services;

public class HomeService : IHomeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountService _accountService;

    public HomeService(IUnitOfWork unitOfWork, IAccountService accountService)
    {
        _unitOfWork = unitOfWork;
        _accountService = accountService;
    }

    public async Task<GetHomeDTO> GetHome()
    {
        // Statistics
        var dashboard = await _unitOfWork.DashboardSettings.GetFirst();
        var settings = await _unitOfWork.Settings.GetFirst();
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

        var accounts = (await _unitOfWork.Accounts.GetAll(x => accountIds.Contains(x.Id))).ToDictionary(a => a.Id, a => a);

        if (accounts.Count() < 4)
            return GetEmptyHome();

        var accountsList = new[]
        {
          accounts[accountIds[0]],
          accounts[accountIds[1]],
          accounts[accountIds[2]],
          accounts[accountIds[3]],
        };

        var balances = new List<AccountBalanceDTO>();

        foreach (var acc in accountsList)
        {
            balances.Add(new AccountBalanceDTO
            {
                AccountName = acc.Name,
                Balance = (await _accountService.GetBalance(acc)).ToString("c")
            });
        }

        var current = DateTime.Now.Date;

        // Line Chart
        var lastPeriods = await GetLastPeriodsBalancesAsync(4);

        // Pie Chart
        var journalsTypesSummary = await GetJournalsTypesSummary(current);

        // Bar Chart
        var lastMonthsJournals = await GetLast12MonthJournalAsync(current);

        // Progress Bar 
        var accountsSet = new HashSet<string>(new string[] 
        { 
          accounts[accountIds[4]].Number, 
          accounts[accountIds[5]].Number 
        });

        (IEnumerable <BudgetAccountDTO> budgetProgress,decimal otherExpenses, decimal availableFunds) =
            await GetBudgetProgressAsync(dashboard,settings,accountsSet , current);

        return new GetHomeDTO
        {
            AccountsSummary = balances,
            LastPeriods = lastPeriods,
            Expenses = lastMonthsJournals.expenses,
            Revenues = lastMonthsJournals.revenues,
            MonthsNames = lastMonthsJournals.months,
            JournalsTypesSummary = journalsTypesSummary,
            AvailableFunds = availableFunds,
            OtherExpenses = otherExpenses,
            OtherExpensesTarget = dashboard.OtherExpensesTarget + dashboard.AddOnExpensesTarget,
            BudgetProgress = budgetProgress,
            DayRate = settings.DefaultDayRate,
            PeriodDays = settings.DefaultPeriodDays.GetValueOrDefault(),
        };
    }

    private async Task<(IEnumerable<BudgetAccountDTO> budgetProgress, decimal otherExpenses, decimal availableFunds)> 
        GetBudgetProgressAsync(DashboardSettings dashboard, Settings settings, HashSet<string> accountSet, DateTime current)
    {
        var currentMonthJournals = (await _unitOfWork.JournalDetail
                   .GetAll(j => (j.Journal.CreatedAt.Month == current.Month && j.Journal.CreatedAt.Year == current.Year), "Journal", "Account"));


        var groupedJournals = currentMonthJournals 
                   .GroupBy(j => j.Account.Number)
                   .Select(j => new { AccountNumber = j.Key, Total = j.Sum(j => j.Debit - j.Credit) });

        var budgetAccounts = await _unitOfWork.BudgetAccounts.GetAll("Account");

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
                     {
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
            ? x.CostCenterId == settings.NotBudgetCostCenter 
            : !excludedAccounts.Any(a => x.Account.Number.StartsWith(a))))
            .Sum(x => x.Debit - x.Credit);


        if (dashboard.ApplyOverBudgetToFunds)
            otherExpenses += overBudget;

        var availableFunds = dashboard.OtherExpensesTarget - otherExpenses + dashboard.AddOnExpensesTarget ;
        
        return (budgetProgress, otherExpenses, availableFunds);
    }
    private async Task<IEnumerable<decimal>> GetLastPeriodsBalancesAsync(int count)
    {
        return (await _unitOfWork.Periods.
                TakeLastOrderBy(count, p => new { p.From, p.TotalAmount }, p => p.From.Date))
                .OrderBy(p => p.From).Select(p => p.TotalAmount);
    }
    private async Task<IEnumerable<decimal>> GetJournalsTypesSummary(DateTime current)
    {
        // Pie Chart

        var journalsTypesSummary = (await _unitOfWork.Journal
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
        // Bar Chart 
        var last11Month = current.AddMonths(-11).Date;
        var firstDayInLast11Month = new DateTime(last11Month.Year, last11Month.Month, 1);
        var lastDayOfCurrentMonth = current.AddMonths(1).AddDays(-1).Date;
        

        List<decimal> expenses = new List<decimal>();
        List<decimal> revenues = new List<decimal>();
        List<string> months = new List<string>();

        var currentYearJournal = (await _unitOfWork.Journal
                .GetAll(j => j.CreatedAt.Date >= firstDayInLast11Month.Date && j.CreatedAt.Date <= lastDayOfCurrentMonth.Date && (j.Type == (byte)JournalTypes.Subtract || j.Type == (byte)JournalTypes.Add)))
                .OrderBy(j => j.CreatedAt);

        var expensesMonthlyGrouped = currentYearJournal.Where(j => j.Type == (byte)JournalTypes.Subtract)
                                                       .GroupBy(j => j.CreatedAt.Month)
                                                       .Select(j => new { j.Key, Total = j.Sum(j => j.Amount * -1) })
                                                       .ToDictionary(j => j.Key, j => j.Total);


        var revenuesMonthlyGrouped = currentYearJournal.Where(j => j.Type == (byte)JournalTypes.Add)
                                                       .GroupBy(j => j.CreatedAt.Month)
                                                       .Select(j => new { j.Key, Total = j.Sum(j => j.Amount) })
                                                       .ToDictionary(j => j.Key, j => j.Total);


        var keys = expensesMonthlyGrouped.Keys;

        foreach (var k in keys)
        {
            expenses.Add(expensesMonthlyGrouped.TryGetValue(k, out decimal expensesValue) ? expensesValue : 0);

            revenues.Add(revenuesMonthlyGrouped.TryGetValue(k, out decimal revenueValue) ? revenueValue : 0);

            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(k);
            months.Add(monthName);
        }

        return (expenses, revenues, months);
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

