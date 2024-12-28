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
                   , "CostCenter", "Account", "Journal"));


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
            Amount = Math.Abs(balance).ToString("c")

        };
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
