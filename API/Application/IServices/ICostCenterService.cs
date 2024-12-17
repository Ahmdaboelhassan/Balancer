using Application.DTO.Request;
using Application.DTO.Response;
using Application.Models;

namespace Application.IServices;
public interface ICostCenterService
{
    Task<GetCostCenter> Get(int id);
    Task<IEnumerable<GetCostCenter>> GetAll();
    Task<ConfirmationResponse> Create(CreateCostCenter model);
    Task<ConfirmationResponse> Edit(CreateCostCenter model);
    Task<ConfirmationResponse> Delete(int id);

}
