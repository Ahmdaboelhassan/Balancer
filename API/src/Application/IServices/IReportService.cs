using Application.DTO.Response;
using Infrastructure.Services;

namespace Application.IServices;
public interface IReportService 
{

    Task<AccountStatement> GetAccountStatement(DateTime from, DateTime to, int accountId, int? costCenterId , bool openingBalance);
    Task<IEnumerable<AccountBalanceDTO>> GetIncomeStatement(DateTime from, DateTime to);
}


