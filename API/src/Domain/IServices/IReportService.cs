using Domain.DTO.Response;

namespace Domain.IServices;
public interface IReportService 
{

    Task<AccountStatement> GetAccountStatement(DateTime? from, DateTime? to, int accountId, int? costCenterId , bool openingBalance);
    Task<IEnumerable<AccountSummaryDTO>> GetIncomeStatement(DateTime from, DateTime to);
    Task<IEnumerable<AccountSummaryDTO>> GetAccountsSummary(DateTime from, DateTime to);
    Task<IEnumerable<AccountSummaryDTO>> GetAccountsOverview(DateTime from, DateTime to);
}


