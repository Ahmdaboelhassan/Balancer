using API.Migrations;
using Application.DTO.Request;
using Application.DTO.Response;
using Application.IRepository;
using Application.IServices;
using Application.Models;
using Azure;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Identity.Client;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Security.Principal;
using System.Text;

namespace Infrastructure.Services;
public class AccountService : IAccountService
{
    private readonly IUnitOfWork _uow;
    public AccountService(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<IEnumerable<GetAccountDTO>> GetAllAccounts()
    {
        var allAccounts = await _uow.Accounts.GetAll("Parent");
        return allAccounts.Select(a =>
            new GetAccountDTO {
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
    public async Task<IEnumerable<SelectItemDTO>> GetAccountSelectList()
    {
        var accounts = await _uow.Accounts.SelectAll(a => !a.IsParent, a => new SelectItemDTO { Id = a.Id, Name = a.Name });
        return accounts.OrderBy(a => a.Name);
    }
    public async Task<GetAccountDTO?> GetAccountById(int id)
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
            IsParent = account.IsParent
        };
    }
    public async Task<ConfirmationResponse> CreateAccount(CreateAccountDTO DTO)
    { 
        var response = await GetAccountNumberAndLevel(DTO);

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
    public async Task<ConfirmationResponse> EditAccount(CreateAccountDTO DTO)
    {
        var account = await _uow.Accounts.Get(DTO.Id);
        if (account == null)
            return new ConfirmationResponse { IsSucceed = false, Message = "Account Not Found" };

        if (account.IsParent && !account.ParentId.Equals(DTO.ParentId)) 
            return new ConfirmationResponse { IsSucceed = false, Message = "Can Not Change Account Number Because It Has Childs" };

        if (DTO.ParentId == account.Id)
            return new ConfirmationResponse { IsSucceed = false, Message = "Account Can Not Be A Parent For Him Self" };

  
        // If Change In Basic Data Only
        if (account.ParentId == null && (DTO.ParentId == 0 || DTO.ParentId == null))  
        {
            account.Name = DTO.Name;
            account.Description = DTO.Description;

            _uow.Accounts.Update(account);
            await _uow.SaveChangesAync();

            return new ConfirmationResponse { IsSucceed = true, Message = "Account Has Been Updated Successfully" };
        }

        int? oldParentId = account?.ParentId;

        var response = await GetAccountNumberAndLevel(DTO);

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
    public async Task<ConfirmationResponse> DeleteAccount(int id)
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

    // Helper Methods
    public int GetAccountNumberDigits(int level, Settings setting)
    {
        return level switch
        {
            2 => setting.LevelTwoDigits,
            3 => setting.LevelThreeDigits,
            4 => setting.LevelFourDigits,
            5 => setting.LevelFiveDigits,
            _ => throw new InvalidOperationException("Invalid level!")
        };

    }
    public async Task<string> GetFirstLevelNumber()
    {
        string nextAccountNumber = "1";
        var numbersInSameLevel = await _uow.Accounts.SelectAll(a => a.Level == 1, a => int.Parse(a.Number));

        if (numbersInSameLevel.Count() > 0)
            nextAccountNumber = (numbersInSameLevel.Max() + 1).ToString();

        return nextAccountNumber;
    }
    public async Task<int> GetNextNumber(int parentId , string parentNumber)
    {
        var childAccountsNums = await _uow.Accounts.SelectAll(a => a.ParentId == parentId, a => int.Parse(a.Number.Substring(parentNumber.Length)));
        return childAccountsNums.Max() + 1;
    }
    public string GetFormattedNumber(int digits , string accNumber ,string parentNumber)
    {
        var number = new StringBuilder();

        number.Append(parentNumber);

        for (int i = 1; i <= digits - accNumber.Length; i++)
            number.Append('0');

        number.Append(accNumber);

        return number.ToString();
    }
   
    private async Task<GetAccountNumberAndLevelResponse> GetAccountNumberAndLevel(CreateAccountDTO DTO)
    {
        var settings = await _uow.Settings.GetFirst();
        if (settings is null)
            return new GetAccountNumberAndLevelResponse{ Message = "There Are No Settings"};

        var accountLevel = 1;
        var accountNumber = string.Empty;

        // Follow Parent Parent Account
        if (DTO.ParentId != null && DTO.ParentId != 0)
        {
            var parent = await _uow.Accounts.Get(DTO.ParentId.Value);
            if (parent is null)
                return new GetAccountNumberAndLevelResponse { Message = "Parent Account Is Not Exist!" };

            accountLevel = parent.Level + 1;
            if (accountLevel > settings.MaxAccountLevel)
                return new GetAccountNumberAndLevelResponse { Message = "Level Can Not Greater Than 4!" }; 

            int levelDigitLength = GetAccountNumberDigits(accountLevel, settings);

            // Parent Has Other Childs 
            if (parent.IsParent)
            {
                int nextAccountNumber = await GetNextNumber(parent.Id, parent.Number);

                var maxNumber = Math.Pow(10, levelDigitLength) - 1;
                if (nextAccountNumber > maxNumber)
                    return new GetAccountNumberAndLevelResponse { Message = $"Number In Level {accountLevel} Should Be {levelDigitLength} Digit!" };

                accountNumber = GetFormattedNumber(levelDigitLength, nextAccountNumber.ToString(), parent.Number);
            }
            else
            {
                // Parent Has No Childs
                accountNumber = GetFormattedNumber(levelDigitLength, "1" ,  parent.Number);

                parent.IsParent = true;
                _uow.Accounts.Update(parent);
            }
        }
        else  // If Not has Parent - The Core 4
        {
            var nextAccountNumber = await GetFirstLevelNumber();

            if (nextAccountNumber.Length > settings.LevelOneDigits)
                return new GetAccountNumberAndLevelResponse { Message = $"Number In Level {accountLevel} Should Be {settings.LevelOneDigits} Digits!" };

            accountNumber = nextAccountNumber;
        }

        return new GetAccountNumberAndLevelResponse { IsSucceed = true, Message = "Account Number Generated Successfully"  , AccountLevel = accountLevel , AccountNumber = accountNumber};

    }

}
