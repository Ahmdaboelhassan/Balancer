
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.IServices;
public interface IAccountService
{
    Task<IEnumerable<GetAccountDTO>> GetAll();
    Task<IEnumerable<AccountingTreeItem>> GetChilds(int id);
    Task<IEnumerable<AccountingTreeItem>> GetPrimaryAccounts();
    Task<IEnumerable<GetAccountDTO>> Search(string criteria);
    Task<IEnumerable<SelectItemDTO>> GetSelectList(Expression<Func<Account, bool>>? criteria = null);
    Task<GetAccountDTO?> GetById(int id);
    Task<decimal> GetBalance(int id);
    Task<ConfirmationResponse> Create(CreateAccountDTO DTO);
    Task<ConfirmationResponse> Edit(CreateAccountDTO DTO);
    Task<ConfirmationResponse> Delete(int id);
}
