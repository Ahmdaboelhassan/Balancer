
using Application.DTO.Request;
using Application.DTO.Response;

namespace Application.IServices;
public interface IAccountService
{
    Task<ConfirmationResponse> CreateAccount(AccountDTO DTO);
    Task<ConfirmationResponse> EditAccount(AccountDTO DTO);
    Task<ConfirmationResponse> DeleteAccount(int id);
    Task<IEnumerable<AccountDTO>> GetAllAccounts();
    Task<AccountDTO?> GetAccountById(int id);
}
