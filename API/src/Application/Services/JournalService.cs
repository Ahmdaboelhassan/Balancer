using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Collections.ObjectModel;

namespace Application.Services;
internal class JournalService : IJournalService
{
    private readonly IUnitOfWork _uow;
    private readonly IAccountService _accountService;
    private readonly ICostCenterService _costCenterService;

    public JournalService(IUnitOfWork uow, IAccountService accountService, ICostCenterService costCenterService)
    {
        _uow = uow;
        _accountService = accountService;
        _costCenterService = costCenterService;
    }
    public async Task<Result<GetJournalDTO>> New(int? periodId)
    {
        var settings = await _uow.Settings.GetFirst();
        if (settings is null)
            return new Result<GetJournalDTO> { Message = "Application Settings Not Found", IsSucceed = false };
        

        return new Result<GetJournalDTO>
        {
            IsSucceed = true,
            Data = new GetJournalDTO()
            {
                Accounts = await _accountService.GetSelectList(a => !a.IsParent && !a.IsArchive),
                CostCenters = await _costCenterService.GetAllSelectList(),
                Code = await GetNextCode(),
                DebitAccountId = settings.DefaultDebitAccount.GetValueOrDefault(),
                CreditAccountId = settings.DefaultCreditAccount.GetValueOrDefault(),
                PeriodId = periodId.GetValueOrDefault(),
                CreatedAt = DateTime.Now,
            }
        };
       
    }
    public async Task<Result<GetJournalDTO>> Get(int id)
    {
        var journal = await _uow.Journal.Get(j => j.Id == id , "JournalDetails", "JournalDetails.Account", "JournalDetails.CostCenter");
        if (journal is null)
        {
            var lastPeriod = await _uow.Periods.GetLastOrderBy(p => p.To);
            int periodId = lastPeriod?.Id ?? 0;

            return await New(periodId);
        }

        var archiveAccount = journal.JournalDetails.FirstOrDefault(x => x.Account.IsArchive);

        if (archiveAccount != null)
            return new Result<GetJournalDTO> { Message = "Can't Load This Journal Because It Contains Archived Account", IsSucceed = false };

        var archiveCostCenter = journal.JournalDetails.FirstOrDefault(x => x.CostCenter != null && x.CostCenter.IsArchived);

        if (archiveCostCenter != null)
            return new Result<GetJournalDTO> { Message = "Can't Load This Journal Because It Contains Archived Cost Center", IsSucceed = false };

        return new Result<GetJournalDTO>
        {
            IsSucceed = true,
            Data = new GetJournalDTO()
            {
                Id = journal.Id,
                CostCenterId = journal.JournalDetails.FirstOrDefault()?.CostCenterId,
                DebitAccountId = journal.JournalDetails.First(d => d.Debit > 0).AccountId,
                CreditAccountId = journal.JournalDetails.First(d => d.Credit > 0).AccountId,
                CreatedAt = journal.CreatedAt,
                Amount = Math.Abs(journal.Amount),
                Type = journal.Type,
                Code = journal.Code,
                Notes = journal.Notes,
                LastUpdatedAt = journal.LastUpdatedAt?.ToString("g"),
                PeriodId = journal.PeriodId,
                Detail = journal.Detail,
                Description = journal.Description,
                CostCenters = await _costCenterService.GetAllSelectList(),
                Accounts = await _accountService.GetSelectList(a => !a.IsParent && !a.IsArchive),
            }
        };

    }
    public async Task<IEnumerable<JournalListItemDTO>> Search(string criteria)
    {
        return (await _uow.Journal.GetAll(a => a.Detail.Contains(criteria) || a.Code.ToString().Contains(criteria) || (a.Description != null && a.Description.Contains(criteria))))
            .OrderByDescending(j => j.CreatedAt).Select(a =>  new JournalListItemDTO
        {
            Id = a.Id,
            Amount = a.Amount,
            Type = a.Type,
            Code = a.Code,
            CreatedAt = a.CreatedAt.ToString("f"),
            Detail = a.Detail,
            Notes = a.Notes,
            periodId = a.PeriodId,
        });
    }
    public async Task<IEnumerable<JournalListItemDTO>> AdvancedSearch(JournalAdvancedSearchDTO DTO)
    {
        var query = _uow.Journal.AsQueryable();

        if (DTO.filterByKey && string.IsNullOrWhiteSpace(DTO.key))
            return Enumerable.Empty<JournalListItemDTO>();

        if (DTO.filterByKey)
        {
            if (DTO.key.StartsWith("C:"))
            {
                var parsedKey = DTO.key.Substring(2, DTO.key.Length - 2).Trim();
                query = query.Where(j => j.Code.ToString().Equals(parsedKey));
            }
            else
            {
                query = query.Where(j => j.Detail.Contains(DTO.key) || j.Code.ToString().Contains(DTO.key) || (j.Description != null && j.Description.Contains(DTO.key)));
            }
        }

        if (DTO.filterByDate && DTO.from.HasValue && DTO.to.HasValue)
            query = query.Where(j => j.CreatedAt.Date >= DTO.from.Value.Date && j.CreatedAt.Date <= DTO.to.Value.Date);

        if (DTO.type != 0)
            query = query.Where(j => j.Type == DTO.type);

        switch (DTO.orderBy)
        {   
            case 1: // By CreatedAt ASC
                query = query.OrderBy(j => j.CreatedAt);
                break;
            case 2: // By CreatedAt DESC
                query = query.OrderByDescending(j => j.CreatedAt);
                break;
            case 3: // By Last Update ASC
                query = query.OrderBy(j => j.LastUpdatedAt);
                break;
            case 4: // By Last Update DESC
                query = query.OrderByDescending(j => j.LastUpdatedAt);
                break;
            case 5: // By Amount ASC
                query = query.OrderBy(j => j.Amount);
                break;
            case 6: // By Amount DESC
                query = query.OrderByDescending(j => j.Amount);
                break;
            case 7: // By Code ASC
                query = query.OrderBy(j => j.Code);
                break;
            case 8: // By Code DESC
                query = query.OrderByDescending(j => j.Code);
                break;
            default:
                query = query.OrderByDescending(j => j.CreatedAt);
                break;
        }

      return await query.Select(a => new JournalListItemDTO
        {
            Id = a.Id,
            Amount = a.Amount,
            Type = a.Type,
            Code = a.Code,
            CreatedAt = a.CreatedAt.ToString("f"),
            Detail = a.Detail,
            Notes = a.Notes,
            periodId = a.PeriodId,
        }).ToListAsync();

    }
    public async Task<IEnumerable<JournalListItemDTO>> GetAll(int page)
    {
        const int pageSize = 20;

        return await _uow.Journal.SelectSome(a => true, a => new JournalListItemDTO
        {
            Id = a.Id,
            Amount = a.Amount,
            Type = a.Type,
            Code = a.Code,
            CreatedAt = a.CreatedAt.ToString("f"),
            Detail = a.Detail,
            Notes = a.Notes
        }, j => j.CreatedAt , page , pageSize);
    }
    public async Task<IEnumerable<JournalListItemDTO>> GetAll(DateTime from, DateTime to)
    {
        return (await _uow.Journal.GetAll(a => a.CreatedAt.Date >= from.Date && a.CreatedAt.Date <= to.Date))
            .OrderByDescending(j => j.CreatedAt).Select( a => new JournalListItemDTO
        {
            Id = a.Id,
            Amount = a.Amount,
            Type = a.Type,
            Code = a.Code,
            CreatedAt = a.CreatedAt.ToString("f"),
            Detail = a.Detail,
            Notes = a.Notes,
            periodId = a.PeriodId,
            
        });
    }
    public async Task<PeriodJournals> GetPeriodJournals(int periodId)
    {
        var currentPeriodId = periodId;

        if (currentPeriodId == 0)
        {
            var lastPeriod = await _uow.Periods.GetLastOrderBy(p => p.To);
            currentPeriodId = lastPeriod?.Id ?? 0;
        }
   
        var period  = await _uow.Periods.Get(p => p.Id == currentPeriodId, "Journals");

        if (period is null)
            return new PeriodJournals();

        return new PeriodJournals
        {
            From = period.From.ToShortDateString(),
            To = period.To.ToShortDateString(),
            Id = period.Id,
            Name = period.Name,
            Journals = period.Journals.OrderBy(j => j.CreatedAt)
            .Select(j => new JournalListItemDTO
            {
                Id = j.Id,
                Amount = j.Amount,
                Type = j.Type,
                Code = j.Code,
                CreatedAt = j.CreatedAt.ToString("f"),
                Detail = j.Detail,
                Notes = j.Notes,
                periodId = j.PeriodId
                
            })
        };
        
    }
    public async Task<int> GetNextCode()
    {
        var isThereJournals = await _uow.Journal.Exists();
        if (isThereJournals)
           return _uow.Journal.GetMaxCode() + 1;

        return 1;
    }

    public async Task<Result<GetJournalDTO>> GetNextJournal(string date)
    {
        var journal = await _uow.Journal.GetNextJournal(date);

        if (journal is null)
            return new Result<GetJournalDTO> { Message = "No Next Journal Found", IsSucceed = false };

        return new Result<GetJournalDTO>
        {
            IsSucceed = true,
            Data = new GetJournalDTO()
            {
                Id = journal.Id,
                CostCenterId = journal.JournalDetails.FirstOrDefault()?.CostCenterId,
                DebitAccountId = journal.JournalDetails.First(d => d.Debit > 0).AccountId,
                CreditAccountId = journal.JournalDetails.First(d => d.Credit > 0).AccountId,
                CreatedAt = journal.CreatedAt,
                Amount = Math.Abs(journal.Amount),
                Type = journal.Type,
                Code = journal.Code,
                Notes = journal.Notes,
                LastUpdatedAt = journal.LastUpdatedAt?.ToString("g"),
                PeriodId = journal.PeriodId,
                Detail = journal.Detail,
                Description = journal.Description,
                CostCenters = await _costCenterService.GetAllSelectList(),
                Accounts = await _accountService.GetSelectList(a => !a.IsParent && !a.IsArchive),
            }
        };
    }
    public async Task<Result<GetJournalDTO>> GetPrevJournal(string date)
    {
        var journal = await _uow.Journal.GetPrevJournal(date);

        if (journal is null)
            return new Result<GetJournalDTO> { Message = "No Previous Journal Found", IsSucceed = false };

        return new Result<GetJournalDTO>
        {
            IsSucceed = true,
            Data = new GetJournalDTO()
            {
                Id = journal.Id,
                CostCenterId = journal.JournalDetails.FirstOrDefault()?.CostCenterId,
                DebitAccountId = journal.JournalDetails.First(d => d.Debit > 0).AccountId,
                CreditAccountId = journal.JournalDetails.First(d => d.Credit > 0).AccountId,
                CreatedAt = journal.CreatedAt,
                Amount = Math.Abs(journal.Amount),
                Type = journal.Type,
                Code = journal.Code,
                Notes = journal.Notes,
                LastUpdatedAt = journal.LastUpdatedAt?.ToString("g"),
                PeriodId = journal.PeriodId,
                Detail = journal.Detail,
                Description = journal.Description,
                CostCenters = await _costCenterService.GetAllSelectList(),
                Accounts = await _accountService.GetSelectList(a => !a.IsParent && !a.IsArchive),
            }
        };
    }


    public async Task<Result<int>> Create(CreateJournalDTO model)
    {
        using (var transaction = _uow.StartTransaction())
        {
            try
            {

                var credit = await _uow.Accounts.Get(model.CreditAccountId);
                var debit = await _uow.Accounts.Get(model.DebitAccountId);

                Period? period = model.PeriodId == 0 ? await _uow.Periods.GetLastOrderBy(p => p.From) : await _uow.Periods.Get(model.PeriodId);

                if (credit == null || debit == null)
                    return new Result<int> { Message = "Credit Account or Debit Account is Wrong" };

                if (credit.IsParent || debit.IsParent)
                    return new Result<int> { Message = "Can Not Make Journal From Using Parent Accounts" };

                if (period == null)
                    return new Result<int> { Message = "Some Thing Wrong While Selecting Journal Period" };

                if (model.CreatedAt.Date > period.To.Date)
                {
                    var difference = model.CreatedAt.Date.Subtract(period.To.Date).TotalDays;
                    return new Result<int> { Message = $"Period Has Been Expired From {difference} Days Ago" };
                }
                
                var code = await GetNextCode();

                var notes = $"{credit.Name} : {debit.Name}";

                var journalType = await GetJournalType(credit.Number , debit.Number);
                var journalAmount = journalType == JournalTypes.Subtract ? model.Amount * -1 : model.Amount;

                if (journalType == JournalTypes.Add || journalType == JournalTypes.Subtract)
                {
                    await _uow.Periods.ExecuteUpdateAsync(p => p.Id == period.Id, e => e.SetProperty(p => p.TotalAmount, p => p.TotalAmount + journalAmount));
                }

                period.LastUpdatedAt = DateTime.Now;

                await _uow.SaveChangesAync();

                var costCenter = await GetCostCenter(debit, model.CostCenterId);

                var newJournal = new Journal
                {
                    Code = code,
                    Notes = notes,
                    CreatedAt = model.CreatedAt,
                    PeriodId = period.Id,
                    Detail = model.Detail,
                    Amount = journalAmount,
                    Type = (byte)journalType,
                    Description = model.Description,
                    LastUpdatedAt = DateTime.Now
                };

                newJournal.JournalDetails = new Collection<JournalDetail>
                {
                    new JournalDetail
                    {
                        AccountId = model.DebitAccountId,
                        CostCenterId = costCenter,
                        Credit = 0,
                        Debit = model.Amount,
                        JournalId = newJournal.Id,
                        Notes = notes,
                    },
                    new JournalDetail
                    {
                        AccountId = model.CreditAccountId,
                        CostCenterId = costCenter,
                        Credit = model.Amount,
                        Debit = 0,
                        JournalId = newJournal.Id,
                        Notes = notes,
                    }
                };

                // Add Journal 
                await _uow.Journal.AddAsync(newJournal);
                await _uow.SaveChangesAync();

                transaction.Commit();

                return new Result<int> { Message = "Journal Has Been Created Successfully", Data = newJournal.Id, IsSucceed = true };
            }
            catch (Exception ex) {
                transaction.Rollback();
                return new Result<int> { Message = $"Exception Happens => {ex.Message}" };
            }
          }
    }
    public async Task<Result<int>> Edit(CreateJournalDTO model)
        {
        using (var transaction = _uow.StartTransaction())
        {
            try
            {
                if (model.Id == 0)
                    return await Create(model);

                var journal = await _uow.Journal.Get(j => j.Id == model.Id, "JournalDetails");

                if (journal is null)
                    return new Result<int> { Message = "Invalid Journal" };

                // Reset Period Value 
                if (journal.Type == (byte)JournalTypes.Add || journal.Type == (byte)JournalTypes.Subtract)
                    await ResetPeriodValueBeforeJournal(journal.PeriodId, journal.Amount);

                var credit = await _uow.Accounts.Get(model.CreditAccountId);
                var debit = await _uow.Accounts.Get(model.DebitAccountId);
                var period = await _uow.Periods.Get(model.PeriodId);

                if (credit is null || debit is null)
                    return new Result<int> { Message = "Credit Account or Debit Account is Wrong" };

                if (credit.IsParent || debit.IsParent)
                    return new Result<int> { Message = "Can Not Make Journal Using Parent Accounts" };

                if (period is null)
                    return new Result<int> { Message = "Some Thing Wrong While Selecting Journal Period" };

                if (model.CreatedAt.Date > period.To.Date)
                {
                    var difference = model.CreatedAt.Date.Subtract(period.To.Date).TotalDays;
                    return new Result<int> { Message = $"Created Date Is Greater Than Period End Date With {difference} Days" };
                }

                var notes = $"{credit.Name} : {debit.Name} (U)";

                var journalType = await GetJournalType(credit.Number, debit.Number);

                var journalAmount = journalType == JournalTypes.Subtract ? model.Amount * -1 : model.Amount;

                if (journal.Type == (byte)JournalTypes.Add || journal.Type == (byte)JournalTypes.Subtract)
                {
                    await _uow.Periods.ExecuteUpdateAsync(p => p.Id == model.PeriodId, e => e.SetProperty(p => p.TotalAmount, p => p.TotalAmount + journalAmount));
                }
                period.LastUpdatedAt = DateTime.Now;


                journal.Notes = notes;
                journal.PeriodId = model.PeriodId;
                journal.Detail = model.Detail;
                journal.Amount = journalAmount;
                journal.Type = (byte)journalType;
                journal.LastUpdatedAt = DateTime.Now;
                journal.Description = model.Description;
                journal.CreatedAt = model.CreatedAt;

                // Delete Old Details
                _uow.JournalDetail.DeleteRange(journal.JournalDetails);
                await _uow.SaveChangesAync();

                // Add New Details 
                var costCenter = model.CostCenterId == 0 ? null : model.CostCenterId;

                journal.JournalDetails = new Collection<JournalDetail>
                {
                    new JournalDetail
                    {
                        AccountId = model.DebitAccountId,
                        CostCenterId = costCenter,
                        Credit = 0,
                        Debit = model.Amount,
                        JournalId = journal.Id,
                        Notes = notes,
                    },
                    new JournalDetail
                    {
                        AccountId = model.CreditAccountId,
                        CostCenterId = costCenter,
                        Credit = model.Amount,
                        Debit = 0,
                        JournalId = journal.Id,
                        Notes = notes,
                    }
                };

                _uow.Journal.Update(journal);

                await _uow.SaveChangesAync();

                transaction.Commit();

                return new Result<int> { Message = "Journal Has Been Updated Successfully", Data = journal.Id ,IsSucceed = true };
            }
            catch (Exception ex) {
                transaction.Rollback();
                return new Result<int> { Message = $"Exception Happens => {ex.Message}" };

            }
        }
    }
    public async Task<ConfirmationResponse> Delete(int id)
    {
        using (var transaction = _uow.StartTransaction())
        {
            try
            {
                var journal = await _uow.Journal.Get(id);

                if (journal is null)
                   return new ConfirmationResponse { Message = "Journal Not Found" };

                journal.IsDeleted = true;
                await _uow.JournalDetail.ExecuteUpdateAsync(d => d.JournalId == journal.Id, e => e.SetProperty(d => d.IsDeleted, true));
                await _uow.SaveChangesAync();


                if (journal.Type == (byte)JournalTypes.Add || journal.Type == (byte)JournalTypes.Subtract)
                {
                    await ResetPeriodValueBeforeJournal(journal.PeriodId, journal.Amount);
                    await _uow.SaveChangesAync();
                }

                transaction.Commit();
               return new ConfirmationResponse { Message = "Journal Deleted Successfully", IsSucceed = true };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new ConfirmationResponse { Message = $"Something Error When Saving => {ex.Message}" };
            }
        }
    }

    public async Task<ConfirmationResponse> AdjustBudget(AdjustBudgetRequest dto)
    {
        if (dto.Amount <= 0)
            return new ConfirmationResponse { Message = "Amount Must Be Greater Than Zero" };

        var setting = await _uow.Settings.GetFirst();
        if (setting is null)
            return new ConfirmationResponse { Message = "Application Has No Settings" };

        var currentCashAccount = await _uow.BudgetAccounts.Get(a => a.AccountId == setting.CurrentCashAccount);

        if (currentCashAccount == null)
            return new ConfirmationResponse { Message = "Current Cash Account Not Found" };

        currentCashAccount.Budget += dto.IsBudgetIncrease ? (dto.Amount * -1) : dto.Amount; // When Increase Budget We Decrease Saving Target Budget  

        _uow.BudgetAccounts.Update(currentCashAccount);

        if (dto.IsBudgetIncrease)
        {
            var dashboard = await _uow.DashboardSettings.GetFirst();

            if (dashboard == null)
                return new ConfirmationResponse { Message = "Dashboard Settings Not Found" };

            dashboard.AddOnExpensesTarget += dto.Amount;
            _uow.DashboardSettings.Update(dashboard);
        }
        
        await _uow.SaveChangesAync();

        return new ConfirmationResponse { Message = "Budget Adjusted Successfully", IsSucceed = true };
    }

    private async Task ResetPeriodValueBeforeJournal(int periodId , decimal amount)
    {
        await _uow.Periods.ExecuteUpdateAsync(p => p.Id == periodId, e => e.SetProperty(p => p.TotalAmount, p => p.TotalAmount + -1 * amount));
        await _uow.SaveChangesAync();
    }
    private async Task<JournalTypes> GetJournalType(string creditNumber , string debitNumber)
    {
        JournalTypes journalType = JournalTypes.Add;

        var setting = await _uow.Settings.GetFirst();
        if (setting == null)
            throw new InvalidDataException("Application Has No Settings");

        var basicAccounts = new int[]
        {
            setting.DrawersAccount.GetValueOrDefault(),
            setting.BanksAccount.GetValueOrDefault(),
            setting.LiabilitiesAccount.GetValueOrDefault(),
            setting.InvestmentAccount.GetValueOrDefault()
        };

        // From Banks and Drawers Then Decrease Balance 
        var accounts = await _uow.Accounts.GetAll(a => basicAccounts.Contains(a.Id));   
        var dict = accounts.ToDictionary(a => a.Id, a => a.Number);

        if (accounts.Count() != basicAccounts.Count()) // All Accounts Exists
              throw new InvalidDataException("There Are No Drawer, Banks Or Liabilities Accounts !");

        Predicate<string> IsDrawerOrBank = (accountNumber) => accountNumber.StartsWith(dict[setting.DrawersAccount.GetValueOrDefault()]) || accountNumber.StartsWith(dict[setting.BanksAccount.GetValueOrDefault()]);

        if (creditNumber.StartsWith(dict[setting.LiabilitiesAccount.GetValueOrDefault()]))
        {
            journalType = JournalTypes.Due;
        }
        else if (debitNumber.StartsWith(dict[setting.InvestmentAccount.GetValueOrDefault()]))
        {
            journalType = JournalTypes.Investment;
        }
        else if (IsDrawerOrBank(creditNumber))
        {
            journalType = IsDrawerOrBank(debitNumber) ? JournalTypes.Forward : JournalTypes.Subtract;
        }
        else if (IsDrawerOrBank(debitNumber))
        {
            journalType = JournalTypes.Add;
        }
       

        return journalType;
    }
    public async Task<int?> GetCostCenter(Account debitAccount, int? costCenterId)
    {
        if (costCenterId.HasValue && costCenterId != 0)
            return costCenterId;

        var setting = await _uow.Settings.GetFirst();

        if (setting is null || setting.NotBudgetCostCenter is null)
            return null;

        var outBudgetAccountIds = new int[]
        {
            setting.ExpensesAccount.GetValueOrDefault(),
            setting.FixedAssetsAccount.GetValueOrDefault()
        };

        var outBudgetAccounts = await _uow.Accounts.GetAll(a => outBudgetAccountIds.Contains(a.Id));
        var outBudgetAccountNumbers = outBudgetAccounts.Select(a => a.Number).ToList();

        if (outBudgetAccounts.Count != outBudgetAccountIds.Count())
            return null;

        var budgetAccounts = await _uow.BudgetAccounts.GetAll("Account");
        var budgetAccountNumbers = budgetAccounts.Select(ba => ba.Account.Number).ToList();


        if (outBudgetAccountNumbers.Any(d => debitAccount.Number.StartsWith(d))
            && !budgetAccountNumbers.Any(d => debitAccount.Number.StartsWith(d))){
            return setting.NotBudgetCostCenter;
        }

        return null; 
    }

}

