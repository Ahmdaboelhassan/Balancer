
using Domain.DTO.Request;
using Domain.DTO.Response;

namespace Domain.IServices;
public interface IPeriodService
{
    Task<IEnumerable<PeriodListItemDTO>> GetAll(int page);
    Task<IEnumerable<PeriodListItemDTO>> GetAll(DateTime From, DateTime To);
    Task<IEnumerable<PeriodListItemDTO>> Search(string criteria);
    Task<IEnumerable<SelectItemDTO>> GetAllSelectList();
    Task<GetPeriodDTO> New();
    Task<GetPeriodDTO> GetLast();
    Task<GetPeriodDTO> GetById(int id);
    Task<ConfirmationResponse> Create(CreatePeriodDTO DTO);
    Task<ConfirmationResponse> Edit(CreatePeriodDTO DTO);
    Task<ConfirmationResponse> Delete(int id);

}
