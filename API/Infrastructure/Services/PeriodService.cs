using Application.DTO.Request;
using Application.DTO.Response;
using Application.IRepository;
using Application.IServices;
using Domain.Models;

namespace Infrastructure.Services;
internal class PeriodService : IPeriodService
{
    private readonly IUnitOfWork _uow;
    public PeriodService(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<IEnumerable<PeriodListItemDTO>> GetAllPeriods(DateOnly? From, DateOnly? To)
    {
        var periodList = await _uow.Periods.SelectAll(p => p.From >= From && p.To <= To, sp => new PeriodListItemDTO
        {
            From = sp.From,
            To = sp.To,
            Name = sp.Name,
            IncreasedBalance = sp.TotalAmount > 0
        });

        if (From != null)
            periodList = periodList.Where(p => p.From >= From);

        if (To != null)
            periodList = periodList.Where(p => p.To <= To);

        return periodList;
    }
    public async Task<IEnumerable<SelectItemDTO>> GetAllPeriodSelectList()
    {
        var periods = await _uow.Periods.GetAll();
        return periods.OrderByDescending(p => p.CreatedAt).Select(p => new SelectItemDTO { Id = p.Id, Name = p.Name });
    }
    public async Task<GetPeriodDetailDTO> GetNewPeriod()
    {
        var DTO = new GetPeriodDetailDTO();
        var LastPeriod = await _uow.Periods.GetLast();

        DTO.DaysCount = 7;
        DTO.From = LastPeriod != null ? LastPeriod.To.AddDays(1) : DateOnly.FromDateTime(DateTime.Now);
        DTO.To = DTO.From.AddDays(DTO.DaysCount);

        return DTO;
    }
    public async Task<GetPeriodDetailDTO> GetPeriodById(int id)
    {
        var period = await _uow.Periods.Get(p => p.Id == id, "Journals");

        if (period == null)
            return null;

        var journals = period.Journals.Select(j => new JournalListItemDTO
        {
            Code = j.Code,
            CreatedAt = j.CreatedAt,
            Detail = j.Detail,
            IncreasedBalance = j.IncreasedBalance,
             Amount = j.Amount,
        });

        var periodDto = new GetPeriodDetailDTO
        {
            CreatedAt = period.CreatedAt,
            DaysCount = period.DaysCount,
            From = period.From,
            To = period.To,
            Id = period.Id,
            Name = period.Name,
            Notes = period.Notes,
            TotalAmount = period.TotalAmount,
            LastUpdatedAt = period.LastUpdatedAt
        };

        return periodDto;
    }
    public async Task<GetPeriodDetailDTO> GetLastPeriod()
    {
        var lastPeriod = await _uow.Periods.GetLast("Journals");
        if (lastPeriod == null)
            return null;

        var journals = lastPeriod.Journals.Select(j => new JournalListItemDTO
        {
            Code = j.Code,
            CreatedAt = j.CreatedAt,
            Detail = j.Detail,
            IncreasedBalance = j.IncreasedBalance,
            Amount = j.Amount,
        });

        var lastPeriodDto = new GetPeriodDetailDTO
        {
            CreatedAt = lastPeriod.CreatedAt,
            DaysCount = lastPeriod.DaysCount,
            From = lastPeriod.From,
            To = lastPeriod.To,
            Id = lastPeriod.Id,
            Name = lastPeriod.Name,
            Notes = lastPeriod.Notes,
            TotalAmount = lastPeriod.TotalAmount,
        };

        if (lastPeriod.LastUpdatedAt.HasValue)
            lastPeriodDto.LastUpdatedAt = lastPeriod.LastUpdatedAt.Value;

        return lastPeriodDto;
    }
    public async Task<ConfirmationResponse> CreatePeriod(SavePeriodDTO DTO)
    {
        var From = DTO.From;
        var To = DTO.From.AddDays(DTO.DaysCount);
        int periodNo = await GetPeriodNumber(From);

        var periodName = $"Period No. {periodNo} In Month {From.Month} In Year {From.Year}";

        var newPeriod = new Period
        {
            Name = periodName,
            From = DTO.From,
            To = To,
            CreatedAt = DateTime.Now,
            DaysCount = DTO.DaysCount,
            Notes = DTO.Notes,
        };
        await _uow.Periods.AddAsync(newPeriod);
        await _uow.SaveChangesAync();
        return new ConfirmationResponse { IsSucceed = true , Message = "Period Has Been Created Successfully"};
    }
    public async Task<ConfirmationResponse> EditPeriod(SavePeriodDTO DTO)
    {
        var From = DTO.From;
        var To = DTO.From.AddDays(DTO.DaysCount);
        int periodNo = await GetPeriodNumber(From);

        var periodName = $"Period No. {periodNo} In Month {From.Month} In Year {From.Year}";

        var newPeriod = new Period
        {
            Name = periodName,
            From = DTO.From,
            To = To,
            CreatedAt = DateTime.Now,
            DaysCount = DTO.DaysCount,
            Notes = DTO.Notes,
        };
        _uow.Periods.Update(newPeriod);
        await _uow.SaveChangesAync();
        return new ConfirmationResponse { IsSucceed = true, Message = "Period Has Been Updated Successfully" };
    }
    public async Task<ConfirmationResponse> DeletePeriod(int id)
    {
        var period = await _uow.Periods.Get(p => p.Id == id, "Journals");

        if (period is null || period.IsDeleted)  
            return new ConfirmationResponse { IsSucceed = false, Message = "Period Is Not Exist!" };

        var JournalIds = period.Journals.Select(j => j.Id);
        await _uow.JournalDetail.ExecuteUpdateAsync(d => JournalIds.Contains(d.JournalId), e => e.SetProperty(d => d.IsDeleted, true));
        await _uow.Journal.ExecuteUpdateAsync(j => j.PeriodId == id, e => e.SetProperty(j => j.IsDeleted ,true));

        period.IsDeleted = true;
        _uow.Periods.Update(period);
        await _uow.SaveChangesAync();
        return  new ConfirmationResponse { IsSucceed = true, Message = "Period Has Been Deleted Successfully" }; ;
    }
    private async Task<int> GetPeriodNumber(DateOnly FromDate)
    {
        int periodsInSameMonth = await _uow.Periods.Count(p => p.From.Month <= FromDate.Month && p.From.Year <= FromDate.Year);
        return periodsInSameMonth == 0 ? 1 : periodsInSameMonth + 1;
    }
}
