using Domain.DTO.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.IServices;
using System.Collections;
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
        var currentAndLastMonthExpenses = await GetCurrentAndLastMonthExpensesAsync(current);

        // Bar Chart
        var currentYearJournal = await GetCurrentYearJournalAsync(current);

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
            CurrentYearExpenses = currentYearJournal.expenses,
            CurrentYearRevenues = currentYearJournal.revenues,
            CurrentAndLastMonthExpenses = currentAndLastMonthExpenses,
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
    private async Task<IEnumerable<decimal>> GetCurrentAndLastMonthExpensesAsync(DateTime current)
    {
        // Pie Chart
        var lastMonth = current.AddMonths(-1);

        var currentExpensesSum = (await _unitOfWork.Journal
               .GetAll(j => j.CreatedAt.Date.Month == current.Month && j.CreatedAt.Date.Year == current.Year && j.Type == (byte)JournalTypes.Subtract)).Sum(j => j.Amount * -1);

        var lastExpensesSum = (await _unitOfWork.Journal
               .GetAll(j => j.CreatedAt.Date.Month == lastMonth.Month && j.CreatedAt.Date.Year == lastMonth.Year && j.Type == (byte)JournalTypes.Subtract)).Sum(j => j.Amount * -1);

        return new List<decimal> { currentExpensesSum, lastExpensesSum };
    }
    private async Task<(List<decimal> expenses, List<decimal> revenues)> GetCurrentYearJournalAsync(DateTime current)
    {
        // Bar Chart 
        List<decimal> currentYearExpenses = new List<decimal>();
        List<decimal> currentYearRevenue = new List<decimal>();

        var currentYearJournal = await _unitOfWork.Journal
                .GetAll(j => j.CreatedAt.Date.Year == current.Year && (j.Type == (byte)JournalTypes.Subtract || j.Type == (byte)JournalTypes.Add));

        var expensesMonthlyGrouped = currentYearJournal.Where(j => j.Type == (byte)JournalTypes.Subtract)
                                                       .GroupBy(j => j.CreatedAt.Month)
                                                       .Select(j => new { j.Key, Total = j.Sum(j => j.Amount * -1) })
                                                       .ToDictionary(j => j.Key, j => j.Total);


        var revenuesMonthlyGrouped = currentYearJournal.Where(j => j.Type == (byte)JournalTypes.Add)
                                                       .GroupBy(j => j.CreatedAt.Month)
                                                       .Select(j => new { j.Key, Total = j.Sum(j => j.Amount) })
                                                       .ToDictionary(j => j.Key, j => j.Total);

        for (int i = 1; i <= 12; i++)
        {
            currentYearExpenses.Add(expensesMonthlyGrouped.TryGetValue(i, out decimal expensesValue) ? expensesValue : 0);

            currentYearRevenue.Add(revenuesMonthlyGrouped.TryGetValue(i, out decimal revenueValue) ? revenueValue : 0);
        }

        return (currentYearExpenses, currentYearRevenue);
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
            CurrentAndLastMonthExpenses = new List<decimal> { 0, 0},
            CurrentYearExpenses = new List<decimal> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            CurrentYearRevenues = new List<decimal> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
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

