using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Collections.ObjectModel;
using static Azure.Core.HttpHeader;

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
  
        var journal = (await _uow.Journal.SelectAll(j => j.Id == id,
            j => new
            {
                Journal = j,
                DebitAccount =  j.JournalDetails.First(d => d.Debit > 0)!.Account,
                CreditAccount = j.JournalDetails.First(d => d.Credit > 0)!.Account,
                CostCentersIds = j.JournalDetails.First().CostCenters.Where(c => c.CostCenter != null).Select(cc => cc.CostCenterId).ToList(),
                HasArchiveAccount = j.JournalDetails.FirstOrDefault(x => x.Account.IsArchive) != null,
                HasArchiveCostCenter = j.JournalDetails.FirstOrDefault(x => x.CostCenters.Any(cc => cc.CostCenter.IsArchived == true)) != null,
            })).FirstOrDefault();

        if (journal is null)
            return await New(null);

        if (journal.HasArchiveAccount)
            return new Result<GetJournalDTO> { Message = "Can't Load This Journal Because It Contains Archived Account", IsSucceed = false };

        if (journal.HasArchiveCostCenter)
            return new Result<GetJournalDTO> { Message = "Can't Load This Journal Because It Contains Archived Cost Center", IsSucceed = false };

        return new Result<GetJournalDTO>
        {
            IsSucceed = true,
            Data = new GetJournalDTO()
            {
                Id = journal.Journal.Id,
                CostCentersIds = journal.CostCentersIds,
                DebitAccountId = journal.DebitAccount?.Id ?? 0,
                CreditAccountId = journal.CreditAccount?.Id ?? 0,
                CreatedAt = journal.Journal.CreatedAt,
                Amount = Math.Abs(journal.Journal.Amount),
                Type = journal.Journal.Type,
                Code = journal.Journal.Code,
                Notes = $"{journal.CreditAccount?.Name ?? ""} -> {journal.DebitAccount?.Name ?? ""}",
                LastUpdatedAt = journal.Journal.LastUpdatedAt?.ToString("f"),
                ActualCreatedAt = journal.Journal.ActualCreatedAt?.ToString("f"),
                PeriodId = journal.Journal.PeriodId,
                Detail = journal.Journal.Detail,
                Description = journal.Journal.Description,
                CostCenters = await _costCenterService.GetAllSelectList(),
                Accounts = await _accountService.GetSelectList(a => !a.IsParent && !a.IsArchive),
            }
        };

    }
    public async Task<IEnumerable<JournalListItemDTO>> Search(string criteria)
    {
        return (await _uow.Journal.GetAll(a => a.Detail.Contains(criteria) || a.Code.ToString().Contains(criteria) || (a.Description != null && a.Description.Contains(criteria))))
            .OrderByDescending(j => j.CreatedAt)
            .Select(a =>  new JournalListItemDTO
            {
                Id = a.Id,
                Amount = a.Amount,
                Type = a.Type,
                Code = a.Code,
                CreatedAt = a.CreatedAt.ToString("f"),
                Detail = a.Detail,
                periodId = a.PeriodId,
                Notes = $"{a.JournalDetails.First(d => d.Credit > 0).Account.Name} -> {a.JournalDetails.First(d => d.Debit > 0).Account.Name}",
                CostCenters = a.JournalDetails.First().CostCenters.Select(cc => cc.CostCenter.Name).ToList(),
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
            case 9: // By Period ASC
                query = query.OrderBy(j => j.PeriodId);
                break;
            case 10: // By Period DESC
                query = query.OrderByDescending(j => j.PeriodId);
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
            periodId = a.PeriodId,
            Notes = $"{a.JournalDetails.First(d => d.Credit > 0).Account.Name} -> {a.JournalDetails.First(d => d.Debit > 0).Account.Name}",
            CostCenters = a.JournalDetails.First().CostCenters.Select(cc => cc.CostCenter.Name).ToList(),
      }).ToListAsync();

    }
   
    public async Task<IEnumerable<JournalListItemDTO>> GetAll(DateTime from, DateTime to)
    {
        var query = _uow.Journal
                .AsQueryable()
                .Where(a => a.CreatedAt.Date >= from.Date && a.CreatedAt.Date <= to.Date)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new JournalListItemDTO
                {
                    Id = a.Id,
                    Amount = a.Amount,
                    Type = a.Type,
                    Code = a.Code,
                    CreatedAt = a.CreatedAt.ToString("f"),
                    Detail = a.Detail,
                    periodId = a.PeriodId,
                    Notes = $"{a.JournalDetails.First(d => d.Credit > 0).Account.Name} -> {a.JournalDetails.First(d => d.Debit > 0).Account.Name}",
                    CostCenters = a.JournalDetails.First().CostCenters.Select(cc => cc.CostCenter.Name).ToList(),
                });

        return await query.ToListAsync();
    }
    public async Task<PeriodJournals> GetPeriodJournals(int periodId)
    {
        var currentPeriodId = periodId;

        if (currentPeriodId == 0)
        {
            var lastPeriod = await _uow.Periods.GetLastOrderBy(p => p.To);
            currentPeriodId = lastPeriod?.Id ?? 0;
        }
   
        var period  = await _uow.Periods
            .SelectAll(p => p.Id == currentPeriodId, period => new PeriodJournals
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
                    Notes = $"{j.JournalDetails.First(d => d.Credit > 0).Account.Name} : {j.JournalDetails.First(d => d.Debit > 0).Account.Name}",
                    periodId = j.PeriodId,
                    CostCenters = j.JournalDetails.First().CostCenters.Select(cc => cc.CostCenter.Name).ToList(),
                })
               }
            );

         if (period is null || !period.Any())
            return new PeriodJournals();

         return period.First();

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
                DebitAccountId = journal.JournalDetails.First(d => d.Debit > 0).AccountId,
                CreditAccountId = journal.JournalDetails.First(d => d.Credit > 0).AccountId,
                CreatedAt = journal.CreatedAt,
                ActualCreatedAt = journal.ActualCreatedAt?.ToString("f"),
                Amount = Math.Abs(journal.Amount),
                Type = journal.Type,
                Code = journal.Code,
                Notes = $"{journal.JournalDetails.First(d => d.Credit > 0).Account.Name} : {journal.JournalDetails.First(d => d.Debit > 0).Account.Name}",
                LastUpdatedAt = journal.LastUpdatedAt?.ToString("f"),
                PeriodId = journal.PeriodId,
                Detail = journal.Detail,
                Description = journal.Description,
                CostCentersIds = journal.JournalDetails.First().CostCenters.Select(cc => cc.CostCenterId).ToList(),
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
                DebitAccountId = journal.JournalDetails.First(d => d.Debit > 0).AccountId,
                CreditAccountId = journal.JournalDetails.First(d => d.Credit > 0).AccountId,
                CreatedAt = journal.CreatedAt,
                Amount = Math.Abs(journal.Amount),
                Type = journal.Type,
                Code = journal.Code,
                Notes = $"{journal.JournalDetails.First(d => d.Credit > 0).Account.Name} : {journal.JournalDetails.First(d => d.Debit > 0).Account.Name}",
                ActualCreatedAt = journal.ActualCreatedAt?.ToString("f"),
                LastUpdatedAt = journal.LastUpdatedAt?.ToString("f"),
                PeriodId = journal.PeriodId,
                Detail = journal.Detail,
                Description = journal.Description,
                CostCentersIds = journal.JournalDetails.First().CostCenters.Select(cc => cc.CostCenterId).ToList(),
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
                    var date = period.To.ToString("dd/MM/yyyy");
                    return new Result<int> { Message = $"Period Has Been Expired From {difference} Days Ago From Day {date}" };
                }

                if (model.CreatedAt.Date < period.From.Date)
                {
                    var difference = period.From.Date.Subtract(model.CreatedAt.Date).TotalDays;
                    var date = period.From.ToString("dd/MM/yyyy");
                    return new Result<int> { Message = $"Cannot Be Created Because It is {difference} Day(s) Before The Start Of The Selected Period From Day {date}" };
                }

                var code = await GetNextCode();

                var journalType = await GetJournalType(credit.Number , debit.Number);
                var journalAmount = journalType == JournalTypes.Subtract ? model.Amount * -1 : model.Amount;

                if (journalType == JournalTypes.Add || journalType == JournalTypes.Subtract)
                {
                    await _uow.Periods.ExecuteUpdateAsync(p => p.Id == period.Id, e => e.SetProperty(p => p.TotalAmount, p => p.TotalAmount + journalAmount));
                }

                period.LastUpdatedAt = DateTime.Now;

                await _uow.SaveChangesAync();

                if (!model.CostCentersIds.Any())
                {
                    var defualtCostCenter = await GetCostCenter(debit);

                    if (defualtCostCenter.HasValue)
                        model.CostCentersIds.Add(defualtCostCenter.Value);
                }



                var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                var egyptTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);

                var newJournal = new Journal
                {
                    Code = code,
                    CreatedAt = model.CreatedAt,
                    PeriodId = period.Id,
                    Detail = model.Detail,
                    Amount = journalAmount,
                    Type = (byte)journalType,
                    Description = model.Description,
                    ActualCreatedAt = egyptTime,
                };

                newJournal.JournalDetails = new Collection<JournalDetail>
                {
                    new JournalDetail
                    {
                        AccountId = model.DebitAccountId,
                        Credit = 0,
                        Debit = model.Amount,
                        JournalId = newJournal.Id,
                        CostCenters = model.CostCentersIds.Select(ccId => new JournalDetailCostCenter { CostCenterId = ccId }).ToList(),
                    },
                    new JournalDetail
                    {
                        AccountId = model.CreditAccountId,
                        Credit = model.Amount,
                        Debit = 0,
                        JournalId = newJournal.Id,
                        CostCenters = model.CostCentersIds.Select(ccId => new JournalDetailCostCenter { CostCenterId = ccId }).ToList(),

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
                    var date = period.To.ToString("dd/MM/yyyy");
                    return new Result<int> { Message = $"Period Has Been Expired From {difference} Days Ago From Day {date}" };
                }

                if (model.CreatedAt.Date < period.From.Date)
                {
                    var difference = period.From.Date.Subtract(model.CreatedAt.Date).TotalDays;
                    var date = period.From.ToString("dd/MM/yyyy");
                    return new Result<int> { Message = $"Cannot Be Created Because It is {difference} Day(s) Before The Start Of The Selected Period From Day {date}" };
                }

                var journalType = await GetJournalType(credit.Number, debit.Number);

                var journalAmount = journalType == JournalTypes.Subtract ? model.Amount * -1 : model.Amount;

                if (journal.Type == (byte)JournalTypes.Add || journal.Type == (byte)JournalTypes.Subtract)
                {
                    await _uow.Periods.ExecuteUpdateAsync(p => p.Id == model.PeriodId, e => e.SetProperty(p => p.TotalAmount, p => p.TotalAmount + journalAmount));
                }
                period.LastUpdatedAt = DateTime.Now;


                var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                var egyptTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);

                journal.PeriodId = model.PeriodId;
                journal.Detail = model.Detail;
                journal.Amount = journalAmount;
                journal.Type = (byte)journalType;
                journal.LastUpdatedAt = egyptTime;
                journal.Description = model.Description;
                journal.CreatedAt = model.CreatedAt;

                // Delete Cost Center Details
                var journalDetailIds = journal.JournalDetails.Select(d => d.Id).ToList();
                var journalDetailCostCenters = await _uow.JournalDetailCostCenters.GetAll(jdc => journalDetailIds.Contains(jdc.JournalDetailId));

                _uow.JournalDetailCostCenters.DeleteRange(journalDetailCostCenters);

                // Delete Old Details
                _uow.JournalDetail.DeleteRange(journal.JournalDetails);

                await _uow.SaveChangesAync();


                journal.JournalDetails = new Collection<JournalDetail>
                {
                    new JournalDetail
                    {
                        AccountId = model.DebitAccountId,
                        Credit = 0,
                        Debit = model.Amount,
                        JournalId = journal.Id,
                        CostCenters = model.CostCentersIds.Select(ccId => new JournalDetailCostCenter { CostCenterId = ccId }).ToList(),
                    },
                    new JournalDetail
                    {
                        AccountId = model.CreditAccountId,
                        Credit = model.Amount,
                        Debit = 0,
                        JournalId = journal.Id,
                        CostCenters = model.CostCentersIds.Select(ccId => new JournalDetailCostCenter { CostCenterId = ccId }).ToList(),
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
                var journal = await _uow.Journal.Get(d => d.Id == id, "JournalDetails");

                if (journal is null)
                   return new ConfirmationResponse { Message = "Journal Not Found" };

                journal.IsDeleted = true;

                // Delete Cost Center Details
                var journalDetailIds = journal.JournalDetails.Select(d => d.Id).ToList();
            
                await _uow.JournalDetailCostCenters.ExecuteUpdateAsync(d => journalDetailIds.Contains(d.JournalDetailId), e => e.SetProperty(d => d.IsDeleted, true));

                await _uow.JournalDetail.ExecuteUpdateAsync(d => d.JournalId == journal.Id, e => e.SetProperty(d => d.IsDeleted, true));

                _uow.Journal.Update(journal);

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
            journalType = IsDrawerOrBank(debitNumber) ? JournalTypes.Add : JournalTypes.Due;

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
    public async Task<int?> GetCostCenter(Account debitAccount)
    {
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

