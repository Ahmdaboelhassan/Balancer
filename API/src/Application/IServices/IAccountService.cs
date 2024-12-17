using Application.DTO.Request;
using Application.DTO.Response;
using Application.Models;

namespace Application.IServices;
public interface IAccountService
{
    Task<IEnumerable<GetAccountDTO>> GetAll();
    Task<IEnumerable<GetAccountDTO>> Search(string criteria);
    Task<IEnumerable<SelectItemDTO>> GetSelectList();
    Task<GetAccountDTO?> GetById(int id);
    Task<ConfirmationResponse> Create(CreateAccountDTO DTO);
    Task<ConfirmationResponse> Edit(CreateAccountDTO DTO);
    Task<ConfirmationResponse> Delete(int id);
}
