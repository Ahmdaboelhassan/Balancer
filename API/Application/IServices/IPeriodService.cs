using Application.DTO.Request;
using Application.DTO.Response;

namespace Application.IServices;
public interface IPeriodService
{
    Task<IEnumerable<PeriodListItemDTO>> GetAllPeriods(DateOnly? From, DateOnly? To);
    Task<IEnumerable<SelectItemDTO>> GetAllPeriodSelectList();
    Task<GetPeriodDetailDTO> GetNewPeriod();
    Task<GetPeriodDetailDTO> GetLastPeriod();
    Task<GetPeriodDetailDTO> GetPeriodById(int id);
    Task<ConfirmationResponse> CreatePeriod(SavePeriodDTO DTO);
    Task<ConfirmationResponse> EditPeriod(SavePeriodDTO DTO);
    Task<ConfirmationResponse> DeletePeriod(int id);

}
