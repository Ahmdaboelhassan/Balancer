using API.Migrations;
using Application.DTO.Request;
using Application.DTO.Response;
using Application.IRepository;
using Application.IServices;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Identity.Client;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Security.Principal;
using System.Text;

namespace Infrastructure.Services;
internal class AccountService : IAccountService
{
    private readonly IUnitOfWork _uow;
    public AccountService(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<IEnumerable<AccountDTO>> GetAllAccounts()
    {
        var allAccounts = await _uow.Accounts.GetAll("Parent");
        return allAccounts.Select(a =>
            new AccountDTO {
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
    public async Task<AccountDTO?> GetAccountById(int id)
    {
        var account = await _uow.Accounts.Get(a => a.Id ==  id , "Parent");

        if (account is null || id == 0)
            return new AccountDTO();

        return new AccountDTO
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
    public async Task<ConfirmationResponse> CreateAccount(AccountDTO DTO)
    {
        int accountLevel = 1;
        string accountNumber = string.Empty;

        var settings = await _uow.SettingsRepo.GetFirst();
        if (settings is null) 
            return new ConfirmationResponse {IsSucceed = false,Message = "There Are No Settings"};

        // Follow Parent Parent Account
        if (DTO.ParentId != null && DTO.ParentId != 0)
        {
            var parentAccount = await _uow.Accounts.Get(DTO.ParentId.Value);
            if (parentAccount is null)
                return new ConfirmationResponse { IsSucceed = false, Message = "Parent Account Is Not Exist!" };

            accountLevel = parentAccount.Level + 1;
            if (accountLevel > 4)
                return new ConfirmationResponse { IsSucceed = false, Message = "Level Can Not Greater Than 4!" };
            
            int levelDigitLength = GetAccountNumberDigits(accountLevel, settings);
            // Parent Has Other Childs 
            if (parentAccount.IsParent)
            {
                var childAccountsNums = await _uow.Accounts.SelectAll(a => a.ParentId == parentAccount.Id, a => long.Parse(a.Number));
                long nextAccountNumber = childAccountsNums.Max() + 1;

                var maxNumber = parentAccount.Number + (Math.Pow(10, levelDigitLength) - 1).ToString();
                if (nextAccountNumber > long.Parse(maxNumber))
                    return new ConfirmationResponse { IsSucceed = false, Message = $"Number In Level {accountLevel} Should Be {levelDigitLength} Digit!" };

                accountNumber = nextAccountNumber.ToString();
            }
            else
            {
                // Parent Has No Childs
                parentAccount.IsParent = true;
                _uow.Accounts.Update(parentAccount);
                accountNumber = GetNumberForFirstChild(levelDigitLength, parentAccount.Number);
            }
        }
        else  // If Not has Parent - The Core 4
        {
            string nextAccountNumber = "1";
            var numbersInSameLevel = await _uow.Accounts.SelectAll(a => a.Level == 1 , a => int.Parse(a.Number));

            if (numbersInSameLevel.Count() > 0)
                 nextAccountNumber  = (numbersInSameLevel.Max() + 1).ToString();

            if (nextAccountNumber.Length > 1)
                return new ConfirmationResponse { IsSucceed = false, Message = "Number In Level 1 Should Be One Digit!" };

            accountNumber = nextAccountNumber;
        }


        var newAccount = new Account
        {
            IsParent = false,
            Description = DTO.Description,
            Name = DTO.Name,
            ParentId = DTO.ParentId > 0 ? DTO.ParentId : null,
            Level = accountLevel,
            Number = accountNumber,
        };

        await _uow.Accounts.AddAsync(newAccount);
        await _uow.SaveChangesAync();
        return new ConfirmationResponse { IsSucceed = true, Message = "Account Has Been Created Successfully" };

    }
    public async Task<ConfirmationResponse> EditAccount(AccountDTO DTO)
    {
        var account = await _uow.Accounts.Get(DTO.Id);
        if (account == null)
            return new ConfirmationResponse { IsSucceed = false, Message = "Account Not Found" };

        if (account.IsParent && !account.ParentId.Equals(DTO.ParentId)) 
            return new ConfirmationResponse { IsSucceed = false, Message = "Can Not Change Account Number Because It Has Childs" };

        if (DTO.ParentId == account.Id)
            return new ConfirmationResponse { IsSucceed = false, Message = "Account Can Not Be A Parent For Him Self" };

        int accountLevel = 1;
        string accountNumber = string.Empty;

        if (account.ParentId == null && (DTO.ParentId == 0 || DTO.ParentId == null))  
        {
            accountLevel = account.Level;
            accountNumber = account.Number;
            DTO.ParentId = null;
        }
        else
        {
            var settings = await _uow.SettingsRepo.GetFirst();
            if (settings is null)
                return new ConfirmationResponse { IsSucceed = false, Message = "There Are No Settings" };

            // Follow Parent Parent Account
            if (DTO.ParentId != null && DTO.ParentId != 0)
            {
                var parentAccount = await _uow.Accounts.Get(DTO.ParentId.Value);
                if (parentAccount is null)
                    return new ConfirmationResponse { IsSucceed = false, Message = "Parent Account Is Not Exist!" };

                accountLevel = parentAccount.Level + 1;
                if (accountLevel > 4)
                    return new ConfirmationResponse { IsSucceed = false, Message = "Level Can Not Greater Than 4!" };

                int levelDigitLength = GetAccountNumberDigits(accountLevel, settings);
                // Parent Has Other Childs 
                if (parentAccount.IsParent)
                {
                    var childAccountsNums = await _uow.Accounts.SelectAll(a => a.ParentId == parentAccount.Id, a => long.Parse(a.Number));
                    long nextAccountNumber = childAccountsNums.Max() + 1;

                    var maxNumber = parentAccount.Number + (Math.Pow(10, levelDigitLength) - 1).ToString();
                    if (nextAccountNumber > long.Parse(maxNumber))
                        return new ConfirmationResponse { IsSucceed = false, Message = $"Number In Level {accountLevel} Should Be {levelDigitLength} Digit!" };

                    accountNumber = nextAccountNumber.ToString();
                }
                else
                {
                    // Parent Has No Childs
                    parentAccount.IsParent = true;
                    _uow.Accounts.Update(parentAccount);
                    accountNumber = GetNumberForFirstChild(levelDigitLength, parentAccount.Number);
                }

            }
            else  // If Not has Parent - The Core 4
            {
                string nextAccountNumber = "1";
                var numbersInSameLevel = await _uow.Accounts.SelectAll(a => a.Level == 1, a => int.Parse(a.Number));

                if (numbersInSameLevel.Count() > 0)
                    nextAccountNumber = (numbersInSameLevel.Max() + 1).ToString();

                if (nextAccountNumber.Length > 1)
                    return new ConfirmationResponse { IsSucceed = false, Message = "Number In Level 1 Should Be One Digit!" };

                accountNumber = nextAccountNumber;
            }
        }
        account.Number = accountNumber;
        account.Name = DTO.Name;
        account.Level = accountLevel;
        account.Description = DTO.Description;
        account.ParentId = DTO.ParentId;

        _uow.Accounts.Update(account);
        await _uow.SaveChangesAync();
        return new ConfirmationResponse { IsSucceed = true, Message = "Account Has Been Updated Successfully" };
    }
    public async Task<ConfirmationResponse> DeleteAccount(int id)
    {
        var account = await _uow.Accounts.Get(id);
        if (account == null)
            return new ConfirmationResponse { IsSucceed = false, Message = "Account Not Found" };

        if (account.IsParent)
            return new ConfirmationResponse { IsSucceed = false, Message = "Can not Remove The Account Because It Has Childs" };

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
    private string GetNumberForFirstChild(int digits , string parentNumber)
    {
        var number = new StringBuilder();
        number.Append(parentNumber);
        for (int i = 1; i < digits; i++)
            number.Append('0');
        number.Append('1');
        return number.ToString();
    }
    private int GetAccountNumberDigits(int level ,Settings setting)
    {
        return level switch
        {
            2 => setting.LevelTwoDigits,
            3 => setting.LevelThreeDigits,
            4 => setting.LevelFourDigits,
            _ => throw new InvalidOperationException("Invalid level!")
        };

    }
}
