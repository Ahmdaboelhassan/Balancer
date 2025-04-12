using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.IServices;
using Microsoft.EntityFrameworkCore;
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
    public async Task<GetJournalDTO?> New(int? periodId)
    {
        var settings = await _uow.Settings.GetFirst();
        if (settings is null)
            return null;

        return new GetJournalDTO()
        {
            Accounts = await _accountService.GetSelectList(a => !a.IsParent),
            CostCenters = await _costCenterService.GetAllSelectList(),
            Code = await GetNextCode(),
            DebitAccountId = settings.DefaultDebitAccount.GetValueOrDefault(),
            CreditAccountId = settings.DefaultCreditAccount.GetValueOrDefault(),
            PeriodId = periodId.GetValueOrDefault(),
            CreatedAt = DateTime.Now,
        };
    }
    public async Task<GetJournalDTO> Get(int id)
    {
        var journal = await _uow.Journal.Get(j => j.Id == id , "JournalDetails");
        if (journal is null)
        {
            var lastPeriod = await _uow.Periods.GetLastOrderBy(p => p.To);
            int periodId = lastPeriod?.Id ?? 0;

            return await New(periodId);
        }

        return new GetJournalDTO()
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
            Accounts = await _accountService.GetSelectList(a => !a.IsParent),
        };

    }
    public async Task<IEnumerable<JournalListItemDTO>> Search(string criteria)
    {
        return (await _uow.Journal.GetAll(a => a.Detail.Contains(criteria) || a.Code.ToString().Contains(criteria)))
            .OrderByDescending(j => j.CreatedAt).Select(a =>  new JournalListItemDTO
        {
            Id = a.Id,
            Amount = a.Amount,
            Type = a.Type,
            Code = a.Code,
            CreatedAt = a.CreatedAt.ToString("D"),
            Detail = a.Detail,
            Notes = a.Notes,
            periodId = a.PeriodId,
        });
    }
    public async Task<IEnumerable<JournalListItemDTO>> AdvancedSearch(string? key ,DateTime from, DateTime to, bool dateFilter, int orderBy, int type)
    {
        var query = _uow.Journal.AsQueryable();

        if (!string.IsNullOrEmpty(key))
            query = query.Where(j => j.Detail.Contains(key) || j.Code.ToString().Contains(key));

        if (type != 0)
            query = query.Where(j => j.Type == type);

        if (dateFilter)
            query = query.Where(j => j.CreatedAt.Date >= from.Date && j.CreatedAt.Date <= to.Date);

        switch (orderBy)
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
            CreatedAt = a.CreatedAt.ToString("D"),
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
            CreatedAt = a.CreatedAt.ToString("D"),
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
            CreatedAt = a.CreatedAt.ToString("D"),
            Detail = a.Detail,
            Notes = a.Notes,
            periodId = a.PeriodId,
            
        });
    }
    public async Task<PeriodJournals> GetPeriodJournals(int periodId)
    {
        var period  = await _uow.Periods.Get(p => p.Id == periodId , "Journals");

        if (period is null)
            return new PeriodJournals();

        return new PeriodJournals
        {
            From = period.From.ToShortDateString(),
            To = period.To.ToShortDateString(),
            Id = period.Id,
            Name = period.Name,
            Journals = period.Journals.OrderByDescending(j => j.CreatedAt)
            .Select(j => new JournalListItemDTO
            {
                Id = j.Id,
                Amount = j.Amount,
                Type = j.Type,
                Code = j.Code,
                CreatedAt = j.CreatedAt.ToString("D"),
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
    public async Task<ConfirmationResponse> Create(CreateJournalDTO model)
    {
        using (var transaction = _uow.StartTransaction())
        {
            try
            {

                var credit = await _uow.Accounts.Get(model.CreditAccountId);
                var debit = await _uow.Accounts.Get(model.DebitAccountId);

                Period? period = model.PeriodId == 0 ? await _uow.Periods.GetLastOrderBy(p => p.From) : await _uow.Periods.Get(model.PeriodId);

                if (credit == null || debit == null)
                    return new ConfirmationResponse { Message = "Credit Account or Debit Account is Wrong" };

                if (credit.IsParent || debit.IsParent)
                    return new ConfirmationResponse { Message = "Can Not Make Journal From Using Parent Accounts" };

                if (period == null)
                    return new ConfirmationResponse { Message = "Some Thing Wrong While Selecting Journal Period" };

                if (model.CreatedAt.Date > period.To.Date)
                {
                    var difference = model.CreatedAt.Date.Subtract(period.To.Date).TotalDays;
                    return new ConfirmationResponse { Message = $"Period Has Been Expired From {difference} Days Ago" };
                }
                
                var code = await GetNextCode();

                var notes = $"{credit.Name} : {debit.Name}";

                var journalType = await GetJournalType(credit.Number , debit.Number);
                var journalAmount = journalType == JournalTypes.Subtract ? model.Amount * -1 : model.Amount;

                if (journalType != JournalTypes.Due && journalType != JournalTypes.Forward)
                {
                    await _uow.Periods.ExecuteUpdateAsync(p => p.Id == model.PeriodId, e => e.SetProperty(p => p.TotalAmount, p => p.TotalAmount + journalAmount));
                }

                period.LastUpdatedAt = DateTime.Now;

                await _uow.SaveChangesAync();

                var costCenter = model.CostCenterId == 0 ? null : model.CostCenterId;
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

                return new ConfirmationResponse { Message = "Journal Has Been Created Successfully", IsSucceed = true };
            }
            catch (Exception ex) {
                transaction.Rollback();
                return new ConfirmationResponse { Message = $"Exception Happens => {ex.Message}" };
            }
          }
    }
    public async Task<ConfirmationResponse> Edit(CreateJournalDTO model)
        {
        using (var transaction = _uow.StartTransaction())
        {
            try
            {
                if (model.Id == 0)
                    return await Create(model);

                var journal = await _uow.Journal.Get(j => j.Id == model.Id, "JournalDetails");

                if (journal is null)
                    return new ConfirmationResponse { Message = "Invalid Journal" };

                // Reset Period Value 
                if (journal.Type != (byte)JournalTypes.Due && journal.Type != (byte)JournalTypes.Forward)
                    await ResetPeriodValueBeforeJournal(journal.PeriodId, journal.Amount);

                var credit = await _uow.Accounts.Get(model.CreditAccountId);
                var debit = await _uow.Accounts.Get(model.DebitAccountId);
                var period = await _uow.Periods.Get(model.PeriodId);

                if (credit is null || debit is null)
                    return new ConfirmationResponse { Message = "Credit Account or Debit Account is Wrong" };

                if (credit.IsParent || debit.IsParent)
                    return new ConfirmationResponse { Message = "Can Not Make Journal Using Parent Accounts" };

                if (period is null)
                    return new ConfirmationResponse { Message = "Some Thing Wrong While Selecting Journal Period" };

                if (model.CreatedAt.Date > period.To.Date)
                {
                    var difference = model.CreatedAt.Date.Subtract(period.To.Date).TotalDays;
                    return new ConfirmationResponse { Message = $"Created Date Is Greater Than Period End Date With {difference} Days" };
                }

                var notes = $"{credit.Name} : {debit.Name} (U)";

                var journalType = await GetJournalType(credit.Number, debit.Number);

                var journalAmount = journalType == JournalTypes.Subtract ? model.Amount * -1 : model.Amount;

                if (journalType != JournalTypes.Due && journalType != JournalTypes.Forward)
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

                return new ConfirmationResponse { Message = "Journal Has Been Updated Successfully", IsSucceed = true };
            }
            catch (Exception ex) {
                transaction.Rollback();
                return new ConfirmationResponse { Message = $"Exception Happens => {ex.Message}" };

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


                if (journal.Type != (byte)JournalTypes.Due && journal.Type != (byte)JournalTypes.Forward)
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

        // From Banks and Drawers Then Decrease Balance 
        var drawerAccount = await _uow.Accounts.Get(setting.DrawersAccount.GetValueOrDefault());
        var banksAccount = await _uow.Accounts.Get(setting.BanksAccount.GetValueOrDefault());
        var liabilitiesAccount = await _uow.Accounts.Get(setting.LiabilitiesAccount.GetValueOrDefault());

        if (drawerAccount is null || banksAccount is null || liabilitiesAccount is null)
                throw new InvalidDataException("There Are No Drawer, Banks Or Liabilities Accounts !");

        Predicate<string> IsDrawerOrBank = (accountNumber) => accountNumber.StartsWith(drawerAccount.Number) || accountNumber.StartsWith(banksAccount.Number);

        if (IsDrawerOrBank(creditNumber))
        {
            journalType = IsDrawerOrBank(debitNumber) ? JournalTypes.Forward : JournalTypes.Subtract;
        }
        else if (creditNumber.StartsWith(liabilitiesAccount.Number))
        {
            journalType = JournalTypes.Due;
        }

        return journalType;
    }

    
}

