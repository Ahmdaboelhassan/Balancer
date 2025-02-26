using Application.DTO.Response;
using Application.IRepository;
using Application.IServices;
using Domain.Models;
using System.Runtime.InteropServices.Marshalling;
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
            From = from.ToString("d"),
            To = to.ToString("d"),
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


        var journals = (await _uow.JournalDetail
            .SelectAll(d => d.Journal.CreatedAt.Date >= from && d.Journal.CreatedAt.Date <= to
               && (d.Account.Number.StartsWith(expensesAccount.Number) || d.Account.Number.StartsWith(RevenuesAccount.Number))
            , d => new { accountId = d.AccountId, AccountNumber = d.Account.Number, AccountName = d.Account.Name, balance = d.Debit - d.Credit , d.Credit , d.Debit}
            , "Account", "Journal"));



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
        var totalRevenues= journals.Where(a => a.AccountNumber.StartsWith(RevenuesAccount.Number)).Sum(a => (a.balance * -1));

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
                Balance = (profit * -1),

            });
        }

        return incomeStatement;
        }

    public async Task<IEnumerable<AccountSummaryDTO>> GetAccountsSummary(DateTime from, DateTime to)
    {

        var journals = (await _uow.JournalDetail
            .SelectAll(d => d.Journal.CreatedAt.Date >= from && d.Journal.CreatedAt.Date <= to
            , d => new { accountId = d.AccountId, AccountNumber = d.Account.Number, AccountName = d.Account.Name, balance = d.Debit - d.Credit, d.Credit, d.Debit }
            , "Account", "Journal"));

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
