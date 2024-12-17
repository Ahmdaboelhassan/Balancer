using API.Migrations;
using Application.DTO.Request;
using Application.DTO.Response;
using Application.IRepository;
using Application.IServices;
using Application.Models;
using Domain;
using Domain.Models;
using static Azure.Core.HttpHeader;

namespace Infrastructure.Services;
internal class JournalService : IJournalService
{
    public readonly IUnitOfWork _uow;
    public JournalService(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<GetJournalDTO> New(int periodId)
    {
        var settings = await _uow.Settings.GetFirst();
        if (settings is null)
            return new GetJournalDTO();

        var code = await GetNextCode();
        var accounts = await  _uow.Accounts.SelectAll(a => !a.IsParent, a => new SelectItemDTO { Id = a.Id, Name = a.Name });
        var costCenters = await _uow.CostCenter.SelectAll(c => true, c => new SelectItemDTO { Id = c.Id, Name = c.Name });

        return new GetJournalDTO()
        {
            Accounts = accounts,
            CostCenters = costCenters,
            Code = code,
            DebitAccountId = settings.DefaultDebitAccount.GetValueOrDefault(),
            CreditAccountId = settings.DefaultCreditAccount.GetValueOrDefault(),
            PeriodId = periodId,
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

        var accounts = await _uow.Accounts.SelectAll(a => !a.IsParent, a => new SelectItemDTO { Id = a.Id, Name = a.Name });
        var costCenters = await _uow.CostCenter.SelectAll(c => true, c => new SelectItemDTO { Id = c.Id, Name = c.Name });

        return new GetJournalDTO()
        {
            Id = journal.Id,
            CostCenterId = journal.JournalDetails.FirstOrDefault()?.CostCenterId,
            DebitAccountId = journal.JournalDetails.First(d => d.Debit > 0).AccountId,
            CreditAccountId = journal.JournalDetails.First(d => d.Credit > 0).AccountId,
            CreatedAt = journal.CreatedAt,
            Amount = journal.Amount,
            Type = journal.Type,
            Code = journal.Code,
            Notes = journal.Notes,
            LastUpdatedAt = journal.LastUpdatedAt,
            PeriodId = journal.PeriodId,
            Detail = journal.Detail,
            Description = journal.Description,
            CostCenters = costCenters,
            Accounts = accounts,
        };

    }
    public async Task<IEnumerable<JournalListItemDTO>> Search(string criteria)
    {
       return  await _uow.Journal.SelectAll(a => a.Detail.Contains(criteria), a =>  new JournalListItemDTO
        {
            Id = a.Id,
            Amount = a.Amount,
            Type = a.Type,
            Code = a.Code,
            CreatedAt = a.CreatedAt,
            Detail = a.Detail,
            Notes = a.Notes
        });
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
            CreatedAt = a.CreatedAt,
            Detail = a.Detail,
            Notes = a.Notes
        }, j => j.CreatedAt , page , pageSize);
    }
    public async Task<IEnumerable<JournalListItemDTO>> GetAll(DateTime from, DateTime to)
    {
        return await _uow.Journal.SelectAll(a => a.CreatedAt.Date >= from.Date && a.CreatedAt.Date <= to.Date  , a => new JournalListItemDTO
        {
            Id = a.Id,
            Amount = a.Amount,
            Type = a.Type,
            Code = a.Code,
            CreatedAt = a.CreatedAt,
            Detail = a.Detail,
            Notes = a.Notes
        });
    }
    public async Task<IEnumerable<JournalListItemDTO>> GetPeriodJournals(int periodId)
    {
        return await _uow.Journal.SelectAll(a => a.PeriodId == periodId, a => new JournalListItemDTO
        {
            Id = a.Id,
            Amount = a.Amount,
            Type = a.Type,
            Code = a.Code,
            CreatedAt = a.CreatedAt,
            Detail = a.Detail,
            Notes = a.Notes
        });
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
                var period = await _uow.Periods.Get(model.PeriodId);

                if (credit == null || debit == null)
                    return new ConfirmationResponse { Message = "Credit Account or Debit Account is Wrong" };

                if (credit.IsParent || debit.IsParent)
                    return new ConfirmationResponse { Message = "Can Not Make Journal From Using Parent Accounts" };

                if (period == null)
                    return new ConfirmationResponse { Message = "Some Thing Wrong While Selecting Journal Period" };

                if (DateTime.Now.Date > period.To.Date)
                {
                    var difference = DateTime.Now.Date.Subtract(period.To.Date).TotalDays;
                    return new ConfirmationResponse { Message = $"Period Has Been Expired From {difference} Days Ago" };
                }
       

                period.LastUpdatedAt = DateTime.Now;
                await _uow.SaveChangesAync();

                var notes = $"Transaction Done From ({credit.Name}) To ({debit.Name})";
                var code = await GetNextCode();


                var setting = await _uow.Settings.GetFirst();
                if (setting == null)
                    return new ConfirmationResponse { Message = $"Please Add Settings" };


                // From Banks and Drawers Then Decrease Balance 
                var drawersAccount = await _uow.Accounts.Get(setting.DrawersAccount.GetValueOrDefault());
                var banksAccount = await _uow.Accounts.Get(setting.BanksAccount.GetValueOrDefault());
                var labilitiesAccount = await _uow.Accounts.Get(setting.LiabilitiesAccount.GetValueOrDefault());

                if (drawersAccount is null || banksAccount is null || labilitiesAccount is null)
                    return new ConfirmationResponse { Message = $"There is No Account For Drawers Or Banks Or Labilities" };

                decimal journalAmount = 0;
                byte journalType = (byte)JournalTypes.Subtract;

                if (credit.Number.StartsWith(drawersAccount.Number) || credit.Number.StartsWith(banksAccount.Number))
                {
                    journalAmount = (-1) * model.Amount;
                    journalType = (byte)JournalTypes.Subtract;
                    await _uow.Periods.ExecuteUpdateAsync(p => p.Id == model.PeriodId, e => e.SetProperty(p => p.TotalAmount, p => p.TotalAmount + journalAmount));

                }
                else if (credit.Number.StartsWith(labilitiesAccount.Number))
                {
                    journalAmount = model.Amount;
                    journalType = (byte)JournalTypes.Due;
                }
                else
                {
                    journalAmount = model.Amount;
                    journalType = (byte)JournalTypes.Add;
                    await _uow.Periods.ExecuteUpdateAsync(p => p.Id == model.PeriodId, e => e.SetProperty(p => p.TotalAmount, p => p.TotalAmount + journalAmount));

                }
                await _uow.SaveChangesAync();


                var newJournal = new Journal
                {
                    Code = code,
                    Notes = notes,
                    CreatedAt = DateTime.Now,
                    PeriodId = model.PeriodId,
                    Detail = model.Detail,
                    Amount = journalAmount,
                    Type = journalType,
                };

                // Add Journal 
                await _uow.Journal.AddAsync(newJournal);
                await _uow.SaveChangesAync();



                // Add Details 
                await _uow.JournalDetail.AddAsync(new JournalDetail
                {
                    AccountId = model.DebitAccountId,
                    CostCenterId = model.CostCenterId,
                    Credit = 0,
                    Debit = model.Amount,
                    JournalId = newJournal.Id,
                    Notes = notes,
                });
                await _uow.SaveChangesAync();

                await _uow.JournalDetail.AddAsync(new JournalDetail
                {
                    AccountId = model.CreditAccountId,
                    CostCenterId = model.CostCenterId,
                    Credit = model.Amount,
                    Debit = 0,
                    JournalId = newJournal.Id,
                    Notes = notes,
                });


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
                if (journal.Type != (byte)JournalTypes.Due)
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
       
                period.LastUpdatedAt = DateTime.Now;
                await _uow.SaveChangesAync();

                var notes = $"Updated Transaction Done From ({credit.Name}) To ({debit.Name})";

                var setting = await _uow.Settings.GetFirst();
                if (setting == null)
                    return new ConfirmationResponse { Message = $"Please Add Settings" };


                // From Banks and Drawers Then Decrease Balance 
                var drawersAccount = await _uow.Accounts.Get(setting.DrawersAccount.GetValueOrDefault());
                var banksAccount = await _uow.Accounts.Get(setting.BanksAccount.GetValueOrDefault());
                var labilitiesAccount = await _uow.Accounts.Get(setting.LiabilitiesAccount.GetValueOrDefault());

                if (drawersAccount is null || banksAccount is null || labilitiesAccount is null)
                    return new ConfirmationResponse { Message = $"There is No Account For Drawers Or Banks Or Labilities" };

                decimal journalAmount = 0;
                byte journalType = (byte)JournalTypes.Subtract;


                if (credit.Number.StartsWith(drawersAccount.Number) || credit.Number.StartsWith(banksAccount.Number))
                {
                    journalAmount = (-1) * model.Amount;
                    journalType = (byte)JournalTypes.Subtract;
                    await _uow.Periods.ExecuteUpdateAsync(p => p.Id == model.PeriodId, e => e.SetProperty(p => p.TotalAmount, p => p.TotalAmount + journalAmount));

                }
                else if (credit.Number.StartsWith(labilitiesAccount.Number))
                {
                    journalAmount = model.Amount;
                    journalType = (byte)JournalTypes.Due;
                }
                else
                {
                    journalAmount = model.Amount;
                    journalType = (byte)JournalTypes.Add;
                    await _uow.Periods.ExecuteUpdateAsync(p => p.Id == model.PeriodId, e => e.SetProperty(p => p.TotalAmount, p => p.TotalAmount + journalAmount));

                }
                await _uow.SaveChangesAync();


                journal.Notes = notes;
                journal.PeriodId = model.PeriodId;
                journal.Detail = model.Detail;
                journal.Amount = journalAmount;
                journal.Type = journalType;
                journal.LastUpdatedAt = DateTime.Now;


                // Add Journal 
                _uow.JournalDetail.DeleteRange(journal.JournalDetails);
                await _uow.SaveChangesAync();

                _uow.Journal.Update(journal);

                // Add Details 
                await _uow.JournalDetail.AddAsync(new JournalDetail
                {
                    AccountId = model.DebitAccountId,
                    CostCenterId = model.CostCenterId,
                    Credit = 0,
                    Debit = model.Amount,
                    JournalId = journal.Id,
                    Notes = notes,
                });
                await _uow.SaveChangesAync();


                await _uow.JournalDetail.AddAsync(new JournalDetail
                {
                    AccountId = model.CreditAccountId,
                    CostCenterId = model.CostCenterId,
                    Credit = model.Amount,
                    Debit = 0,
                    JournalId = journal.Id,
                    Notes = notes,
                });

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
        using var transaction = _uow.StartTransaction();
        try
        {
            var journal = await _uow.Journal.Get(id);

            if (journal is null)
                new ConfirmationResponse { Message = "Journal Not Found" };

            journal.IsDeleted = true;
            await _uow.JournalDetail.ExecuteUpdateAsync(d => d.JournalId == journal.Id, e => e.SetProperty(d => d.IsDeleted, true));
            await _uow.SaveChangesAync();


            if (journal.Type != (byte)JournalTypes.Due)
            {
                await ResetPeriodValueBeforeJournal(journal.PeriodId, journal.Amount);
                await _uow.SaveChangesAync();
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return new ConfirmationResponse { Message = $"Something Error When Saving => {ex.Message}" };
        }
        return new ConfirmationResponse { Message = "Journal Deleted Successfully"  , IsSucceed = true};
    }
    private async Task ResetPeriodValueBeforeJournal(int periodId , decimal amount)
    {
        await _uow.Periods.ExecuteUpdateAsync(p => p.Id == periodId, e => e.SetProperty(p => p.TotalAmount, p => p.TotalAmount + (-1 * amount)));
        await _uow.SaveChangesAync();
    }
}

