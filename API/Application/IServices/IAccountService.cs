using Application.DTO.Request;
using Application.DTO.Response;
using Application.Models;

namespace Application.IServices;
public interface IAccountService
{
    Task<IEnumerable<GetAccountDTO>> GetAllAccounts();
    Task<IEnumerable<SelectItemDTO>> GetAccountSelectList();
    Task<GetAccountDTO?> GetAccountById(int id);
    Task<ConfirmationResponse> CreateAccount(CreateAccountDTO DTO);
    Task<ConfirmationResponse> EditAccount(CreateAccountDTO DTO);
    Task<ConfirmationResponse> DeleteAccount(int id);
}
