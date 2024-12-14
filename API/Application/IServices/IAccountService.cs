
using Application.DTO.Request;
using Application.DTO.Response;

namespace Application.IServices;
public interface IAccountService
{
    Task<IEnumerable<AccountDTO>> GetAllAccounts();
    Task<IEnumerable<SelectItemDTO>> GetAccountSelectList();
    Task<AccountDTO?> GetAccountById(int id);
    Task<ConfirmationResponse> CreateAccount(AccountDTO DTO);
    Task<ConfirmationResponse> EditAccount(AccountDTO DTO);
    Task<ConfirmationResponse> DeleteAccount(int id);
}
