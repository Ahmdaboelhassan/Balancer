
using Domain.DTO.Request;
using Domain.DTO.Response;

namespace Domain.IServices;
public interface ICostCenterService
{
    Task<GetCostCenter> Get(int id);
    Task<IEnumerable<GetCostCenter>> GetAll();
    Task<IEnumerable<SelectItemDTO>> GetAllSelectList();
    Task<ConfirmationResponse> Create(CreateCostCenter model);
    Task<ConfirmationResponse> Edit(CreateCostCenter model);
    Task<ConfirmationResponse> Delete(int id);

}
