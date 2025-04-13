using Domain.DTO.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.IServices;
using System.Text;

namespace Application.Services;

public class HomeService : IHomeService
{
    private readonly IUnitOfWork _unitOfWork;

    public HomeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetHomeDTO> GetHome()
    {
        // Statistics
        var dashboard = await _unitOfWork.DashboardAccounts.GetFirst();
        if (dashboard == null)
            return GetEmptyHome();

        var accountIds = new[]
        {
            dashboard.Account1,
            dashboard.Account2,
            dashboard.Account3,
            dashboard.Account4,
        };

        var accounts = (await _unitOfWork.Accounts.GetAll(x => accountIds.Contains(x.Id))).ToDictionary(a => a.Id , a => a) ;
        if (accounts.Count() < accountIds.Length)
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
                Balance = (await GetBalance(acc)).ToString("c")
            });
        }


        // Line Chart
        var lastPeriods = (await _unitOfWork.Periods.
                TakeLastOrderBy(4, p => new { p.From, p.TotalAmount }, p => p.From.Date))
                .OrderBy(p => p.From).Select(p => p.TotalAmount);
                

        // Pie Chart
        var current = DateTime.Now.Date;

        var lastMonth = current.AddMonths(-1);

        var currentExpensesSum = (await _unitOfWork.Journal
               .GetAll(j => j.CreatedAt.Date.Month == current.Month && j.CreatedAt.Date.Year == current.Year && j.Type == (byte)JournalTypes.Subtract)).Sum(j => j.Amount * -1);

        var lastExpensesSum = (await _unitOfWork.Journal
               .GetAll(j => j.CreatedAt.Date.Month == lastMonth.Month && j.CreatedAt.Date.Year == lastMonth.Year && j.Type == (byte)JournalTypes.Subtract)).Sum(j => j.Amount * -1);


        var currentAndLastMonthExpenses = new List<decimal> {currentExpensesSum ,lastExpensesSum };
        
        // Bar Chart 
        List<decimal> currentYearExpenses = new List<decimal>();
        List<decimal> currentYearRevenue = new List<decimal>();

        var currentYearJournal = await _unitOfWork.Journal
                .GetAll(j => j.CreatedAt.Date.Year == current.Year && (j.Type == (byte)JournalTypes.Subtract || j.Type == (byte)JournalTypes.Add));

        var expensesMonthlyGrouped = currentYearJournal.Where(j => j.Type == (byte)JournalTypes.Subtract)
                                                       .GroupBy(j => j.CreatedAt.Month)
                                                       .Select(j => new { j.Key, Total = j.Sum(j => j.Amount * -1) })
                                                       .ToDictionary(j => j.Key , j => j.Total);


        var revenuesMonthlyGrouped = currentYearJournal.Where(j => j.Type == (byte)JournalTypes.Add)
                                                       .GroupBy(j => j.CreatedAt.Month)
                                                       .Select(j => new { j.Key, Total = j.Sum(j => j.Amount )})
                                                       .ToDictionary(j => j.Key, j => j.Total);

        for (int i = 1; i <= 12; i++)
        {
            currentYearExpenses.Add(expensesMonthlyGrouped.TryGetValue(i, out decimal expensesValue) ? expensesValue : 0);

            currentYearRevenue.Add(revenuesMonthlyGrouped.TryGetValue(i, out decimal revenueValue) ? revenueValue : 0);
        }

        return new GetHomeDTO
        {
            AccountsSummary = balances,
            LastPeriods = lastPeriods,
            CurrentYearExpenses = currentYearExpenses,
            CurrentYearRevenues = currentYearRevenue,
            CurrentAndLastMonthExpenses = currentAndLastMonthExpenses,
        };
    }

    public GetHomeDTO GetEmptyHome()
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
           
        };
    }

    public async Task<decimal> GetBalance(Account account)
    {
        if (account is null)
            return 0;

        return (await _unitOfWork.JournalDetail.SelectAll(d => d.Account.Number.StartsWith(account.Number), d => d.Debit - d.Credit, "Account")).Sum();
        
    }
}
