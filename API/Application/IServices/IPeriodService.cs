using Application.DTO.Request;
using Application.DTO.Response;

namespace Application.IServices;
public interface IPeriodService
{
    Task<ConfirmationResponse> CreatePeriod(SavePeriodDTO DTO);
    Task<ConfirmationResponse> EditPeriod(SavePeriodDTO DTO);
    Task<ConfirmationResponse> DeletePeriod(int id);
    Task<IEnumerable<PeriodListItemDTO>> GetAllPeriods(DateOnly From, DateOnly To);
    Task<GetPeriodDetailDTO> GetPeriodById(int id);
    Task<GetPeriodDetailDTO> GetLastPeriod();
    Task<GetPeriodDetailDTO> GetNewPeriod();
}
