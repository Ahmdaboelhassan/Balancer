using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.IRepository;
using Domain.IServices;
using System.Text;

namespace Application.Services;

class AccountNumberService : IAccountNumberService
{
    private readonly IUnitOfWork _uow;
    
    public AccountNumberService(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<GetAccountNumberAndLevelResponse> GetAccountNumberAndLevel(CreateAccountDTO DTO)
    {
        var settings = await _uow.Settings.GetFirst();
        if (settings is null)
            return new GetAccountNumberAndLevelResponse { Message = "There Are No Settings" };

        var accountLevel = 1;
        var accountNumber = string.Empty;

        // Follow Parent Parent Account
        if (DTO.ParentId != null && DTO.ParentId != 0)
        {
            var parent = await _uow.Accounts.Get(DTO.ParentId.Value);
            if (parent is null)
                return new GetAccountNumberAndLevelResponse { Message = "Parent Account Is Not Exist!" };

            var parentHasJournals = await _uow.JournalDetail.Exists(d => d.AccountId == parent.Id);
            if (parentHasJournals)
                return new GetAccountNumberAndLevelResponse { Message = "This Account Can Not Has A Child Because It Has Transactions" };

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
                accountNumber = GetFormattedNumber(levelDigitLength, "1", parent.Number);

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

        return new GetAccountNumberAndLevelResponse { IsSucceed = true, Message = "Account Number Generated Successfully", AccountLevel = accountLevel, AccountNumber = accountNumber };

    }

    private int GetAccountNumberDigits(int level, Settings setting)
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
    private async Task<string> GetFirstLevelNumber()
    {
        string nextAccountNumber = "1";
        var numbersInSameLevel = await _uow.Accounts.SelectAll(a => a.Level == 1, a => int.Parse(a.Number));

        if (numbersInSameLevel.Count() > 0)
            nextAccountNumber = (numbersInSameLevel.Max() + 1).ToString();

        return nextAccountNumber;
    }
    private async Task<int> GetNextNumber(int parentId, string parentNumber)
    {
        var childAccountsNums = await _uow.Accounts.SelectAll(a => a.ParentId == parentId, a => int.Parse(a.Number.Substring(parentNumber.Length)));
        return childAccountsNums.Max() + 1;
    }

    private string GetFormattedNumber(int digits, string accNumber, string parentNumber)
    {
        var number = new StringBuilder();

        number.Append(parentNumber);

        for (int i = 1; i <= digits - accNumber.Length; i++)
            number.Append('0');

        number.Append(accNumber);

        return number.ToString();
    }
}
