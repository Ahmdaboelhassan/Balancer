using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.IRepository;
using Domain.IServices;
using System.Linq.Expressions;
using System.Text;

namespace Application.Services;
public class AccountService : IAccountService
{
    private readonly IUnitOfWork _uow;
    private readonly IAccountNumberService _accountNumberService;
    public AccountService(IUnitOfWork uow, IAccountNumberService accountNumberService)
    {
        _uow = uow;
        _accountNumberService = accountNumberService;
    }
    public async Task<IEnumerable<GetAccountDTO>> GetAll()
    {
        return (await _uow.Accounts.GetAll("Parent"))
            .OrderBy(a => a.Level).Select(a =>
            new GetAccountDTO
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                ParentId = a.ParentId,
                AccountNumber = a.Number,
                ParentNumber = a.Parent?.Number,
                ParentName = a.Parent?.Name,
                IsParent = a.IsParent,
            });
    }
    public async Task<IEnumerable<AccountingTreeItem>> GetPrimaryAccounts() {

        return await _uow.Accounts.SelectAll(a => a.ParentId == null, a => new AccountingTreeItem { Name = a.Name, Number = a.Number, Id = a.Id , Level = a.Level });
    }
    public async Task<IEnumerable<AccountingTreeItem>> GetChilds(int id)
    {
        return await _uow.Accounts.SelectAll(a => a.ParentId == id, a => new AccountingTreeItem { Name = a.Name, Number = a.Number, Id = a.Id , Level = a.Level });

    }
    public async Task<IEnumerable<GetAccountDTO>> Search(string criteria) {
        var allAccounts = await _uow.Accounts.GetAll(a => a.Name.Contains(criteria) ,"Parent");
        return allAccounts.Select(a =>
            new GetAccountDTO
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                ParentId = a.ParentId,
                AccountNumber = a.Number,
                ParentNumber = a.Parent?.Number,
                ParentName = a.Parent?.Name,
                IsParent = a.IsParent
            });
    }
    public async Task<IEnumerable<SelectItemDTO>> GetSelectList(Expression<Func<Account, bool>>? criteria = null)
    {
        return (await _uow.Accounts.SelectAll(criteria is null ? a => true : criteria , a => new SelectItemDTO { Id = a.Id, Name = a.Name })).OrderBy(a => a.Name); 
    }
    public async Task<GetAccountDTO?> GetById(int id)
    {
        var account = await _uow.Accounts.Get(a => a.Id ==  id , "Parent");

        if (account is null || id == 0)
            return new GetAccountDTO();

        return new GetAccountDTO
        {
            Id = account.Id,
            Name = account.Name,
            Description = account.Description,
            ParentId = account.ParentId,
            AccountNumber = account.Number,
            ParentNumber = account.Parent?.Number,
            ParentName = account.Parent?.Name, 
            IsParent = account.IsParent,
            Accounts = await GetSelectList(a => a.Id != id)
        };
    }
    public async Task<ConfirmationResponse> Create(CreateAccountDTO DTO)
    { 

        var response = await _accountNumberService.GetAccountNumberAndLevel(DTO);

        if (!response.IsSucceed)
            return new ConfirmationResponse { Message = response.Message};

        var newAccount = new Account
        {
            IsParent = false,
            Description = DTO.Description,
            Name = DTO.Name,
            ParentId = DTO.ParentId > 0 ? DTO.ParentId : null,
            Level = response.AccountLevel,
            Number = response.AccountNumber,
        };

        await _uow.Accounts.AddAsync(newAccount);
        await _uow.SaveChangesAync();
        return new ConfirmationResponse {IsSucceed = true ,Message = "Account Has Been Created Successfully" }; ;

    }
    public async Task<ConfirmationResponse> Edit(CreateAccountDTO DTO)
    {
        var account = await _uow.Accounts.Get(DTO.Id);
        if (account == null)
            return new ConfirmationResponse { IsSucceed = false, Message = "Account Not Found" };

        if (account.IsParent && !account.ParentId.Equals(DTO.ParentId)) 
            return new ConfirmationResponse { IsSucceed = false, Message = "Can Not Change Account Number Because It Has Childs" };

        if (DTO.ParentId == account.Id)
            return new ConfirmationResponse { IsSucceed = false, Message = "Account Can Not Be A Parent For Him Self" };

        int? oldParentId = account?.ParentId;

        // If Change In Basic Data Only
        if (oldParentId.Equals(DTO.ParentId))  
        {
            account.Name = DTO.Name;
            account.Description = DTO.Description;

            _uow.Accounts.Update(account);
            await _uow.SaveChangesAync();

            return new ConfirmationResponse { IsSucceed = true, Message = "Account Has Been Updated Successfully" };
        }


        var response = await _accountNumberService.GetAccountNumberAndLevel(DTO);

        if (!response.IsSucceed)
            return new ConfirmationResponse { Message = response.Message };

        account.ParentId = DTO.ParentId;
        account.Name = DTO.Name;
        account.Description = DTO.Description;
        account.Level = response.AccountLevel;
        account.Number = response.AccountNumber;

        _uow.Accounts.Update(account);
        await _uow.SaveChangesAync();

        if (!await _uow.Accounts.Exists(a => a.ParentId == oldParentId && a.Id != account.Id))
        {
            await _uow.Accounts.ExecuteUpdateAsync(a => a.Id == oldParentId , e => e.SetProperty(a => a.IsParent , false));
            await _uow.SaveChangesAync();
        }

        return new ConfirmationResponse { IsSucceed = true, Message = "Account Has Been Updated Successfully" };
    }
    public async Task<ConfirmationResponse> Delete(int id)
    {
        var account = await _uow.Accounts.Get(id);
        if (account == null)
            return new ConfirmationResponse {Message = "Account Not Found" };

        if (account.IsParent)
            return new ConfirmationResponse {Message = "Can not Remove The Account Because It Has Childs" };

        if(await _uow.JournalDetail.Exists(d => d.AccountId == id))
            return new ConfirmationResponse {Message = "Can not Remove The Account Because There Are Transactions On It" };


        account.IsDeleted = true;
        _uow.Accounts.Update(account);

        if (account.ParentId != null)
        {
            var parentAccountHasOtherChild = await _uow.Accounts.Exists(a => a.ParentId == account.ParentId && !a.IsDeleted && a.Id != account.Id);

            if (!parentAccountHasOtherChild)
            {
                var parentAccount = await _uow.Accounts.Get(account.ParentId.Value);
                parentAccount.IsParent = false;
                _uow.Accounts.Update(parentAccount);
            }
        }

        await _uow.SaveChangesAync();

        return new ConfirmationResponse { IsSucceed = true, Message = "Account Deleted Successfully" };
    }
    public async Task<decimal> GetBalance(int accId)
    {
        var account = await _uow.Accounts.Get(accId);
        if (account == null)
            return 0;

        var journals = await _uow.JournalDetail
                   .SelectAll(d => d.Account.Number.StartsWith(account.Number), d => d.Debit - d.Credit, "Account");

        return journals.Sum();

    }
    public async Task<decimal> GetBalance(Account account)
    {
        if (account is null) return 0;

        return (await _uow.JournalDetail.SelectAll(d => d.Account.Number.StartsWith(account.Number), d => d.Debit - d.Credit, "Account")).Sum();
    }
}
