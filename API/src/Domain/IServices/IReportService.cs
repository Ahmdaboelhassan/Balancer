using Domain.DTO.Response;
using Domain.Enums;

namespace Domain.IServices;
public interface IReportService 
{

    Task<AccountStatement> GetAccountStatement(DateTime? from, DateTime? to, int accountId, int? costCenterId , bool openingBalance);
    Task<IEnumerable<AccountSummaryDTO>> GetIncomeStatement(DateTime from, DateTime to);
    Task<IEnumerable<AccountSummaryDTO>> GetAccountsSummary(DateTime from, DateTime to);
    Task<IEnumerable<AccountSummaryDTO>> GetAccountsOverview(DateTime from, DateTime to, int? maxLevel);
    Task<AccountComparerDTO> GetAccountComparer(DateTime? from, DateTime? to, int accountId, int? costCenterId, AccountComparerGroups groupType);
}


