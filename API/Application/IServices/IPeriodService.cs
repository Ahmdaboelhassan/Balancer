using Application.DTO.Request;
using Application.DTO.Response;
using Application.Models;

namespace Application.IServices;
public interface IPeriodService
{
    Task<IEnumerable<PeriodListItemDTO>> GetAllPeriods(DateTime? From, DateTime? To);
    Task<IEnumerable<SelectItemDTO>> GetAllPeriodSelectList();
    Task<GetPeriodDTO> GetNewPeriod();
    Task<GetPeriodDTO> GetLastPeriod();
    Task<GetPeriodDTO> GetPeriodById(int id);
    Task<ConfirmationResponse> CreatePeriod(CreatePeriodDTO DTO);
    Task<ConfirmationResponse> EditPeriod(CreatePeriodDTO DTO);
    Task<ConfirmationResponse> DeletePeriod(int id);

}
