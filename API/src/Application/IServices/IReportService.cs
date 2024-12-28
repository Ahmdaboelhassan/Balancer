using Application.DTO.Response;

namespace Application.IServices;
public interface IReportService 
{

    Task<AccountStatement> GetAccountStatement(DateTime from, DateTime to, int accountId, int? costCenterId , bool openingBalance);

}


