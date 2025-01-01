using Application.DTO.Response;
using Application.IRepository;
using Application.IServices;
using Domain.Models;
using System.Text;

namespace Infrastructure.Services;
public class ReportService : IReportService
{
    private readonly IUnitOfWork _uow;
    public ReportService(IUnitOfWork uow) => _uow = uow;

    public async Task<AccountStatement> GetAccountStatement(DateTime from, DateTime to, int accountId, int? costCenterId , bool openingBalance)
    {
        var account = await _uow.Accounts.Get(accountId);
        if (account == null)
            return GetEmptyAccountStatement();

        var journals = (await _uow.JournalDetail
                   .GetAll(d =>
                        d.Account.Number.StartsWith(account.Number)
                     && d.Journal.CreatedAt.Date >= from.Date
                     && d.Journal.CreatedAt.Date <= to.Date
                     && (costCenterId.HasValue ? d.CostCenterId == costCenterId : true)
                   , "CostCenter", "Account", "Journal")).OrderBy(d => d.Journal.CreatedAt);


        var journalsLinkedList = new LinkedList<AccountStatementDetail>();

        decimal balance = 0;
        if (openingBalance)
        {
            var openingJournals = (await _uow.JournalDetail
                    .GetAll(d =>
                         d.Account.Number.StartsWith(account.Number)
                       && d.Journal.CreatedAt < from.Date
                      && (costCenterId.HasValue ? d.CostCenterId == costCenterId : true)
                    , "Account", "Journal"));

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
        return new AccountStatement()
        {
            AccountType = balance > 0 ? "Debit" : "Credit",
            Details = journalsLinkedList,
            AccountName = account.Name,
            From = from.ToString("d"),
            To = to.ToString("d"),
            Amount = Math.Abs(balance).ToString("c")
        };
    }

    public async Task<IEnumerable<AccountBalanceDTO>> GetIncomeStatement(DateTime from, DateTime to)
    {
        var settings = await _uow.Settings.GetFirst();

        if (settings is null)
            return Enumerable.Empty<AccountBalanceDTO>();

        var expensesAccount = await _uow.Accounts.Get(settings.ExpensesAccount.GetValueOrDefault());
        var RevenuesAccount = await _uow.Accounts.Get(settings.RevenueAccount.GetValueOrDefault());

        if (RevenuesAccount is null || expensesAccount is null)
            return Enumerable.Empty<AccountBalanceDTO>();


        var journals = await _uow.JournalDetail
            .SelectAll(d => d.Journal.CreatedAt.Date >= from && d.Journal.CreatedAt.Date <= to 
               && (d.Account.Number.StartsWith(expensesAccount.Number) || d.Account.Number.StartsWith(RevenuesAccount.Number))
            , d => new {accountId = d.AccountId , AccountNumber = d.Account.Number , AccountName = d.Account.Name , balance = d.Debit - d.Credit}
            , "Account", "Journal");


        var incomeStatement = journals.GroupBy(j => j.accountId)
                .Select(d => new AccountBalanceDTO { AccountName = d.First().AccountName, Balance = d.Sum(d => d.balance).ToString("c") }).ToList();


        var totalExpenses = journals.Where(a => a.AccountNumber.StartsWith(expensesAccount.Number)).Sum(a => a.balance);
        var totalRevenues= journals.Where(a => a.AccountNumber.StartsWith(RevenuesAccount.Number)).Sum(a => (a.balance * -1));

        var profit = totalRevenues - totalExpenses;

        if (profit > 0)
        {
            incomeStatement.Add(new AccountBalanceDTO
            {
                AccountName = "Excess of Revenues Over Expenses",
                Balance = profit.ToString("c")
            });

        }
        else
        {
            incomeStatement.Add(new AccountBalanceDTO
            {
                AccountName = "Excess of Expenses Over Revenues",
                Balance = (profit * -1).ToString("c")
            });
        }

        return incomeStatement;
        }

    private AccountStatement GetEmptyAccountStatement()
    {
        return new AccountStatement()
        {
            AccountType = "",
            Amount = 0.ToString("c"),
            Details = Enumerable.Empty<AccountStatementDetail>()
        };
    }
}
