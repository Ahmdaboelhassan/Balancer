using Application.DTO.Request;
using Application.DTO.Response;
using Application.IRepository;
using Application.IServices;
using Application.Models;
using Domain.Models;

namespace Infrastructure.Services;
internal class PeriodService : IPeriodService
{
    private readonly IUnitOfWork _uow;
    public PeriodService(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<IEnumerable<PeriodListItemDTO>> GetAllPeriods(DateTime? From, DateTime? To)
    {
        var periodList = await _uow.Periods.SelectAll( a => true , sp => new PeriodListItemDTO
        {
            Id = sp.Id,
            Name = sp.Name,
            From = sp.From,
            To = sp.To,
            TotalAmount = sp.TotalAmount,
            IncreasedBalance = sp.TotalAmount > 0,
            
        });

        if (From != null)
            periodList = periodList.Where(p => p.From >= From.Value.Date);

        if (To != null)
            periodList = periodList.Where(p => p.To <= To.Value.Date);

        return periodList;
    }
    public async Task<IEnumerable<SelectItemDTO>> GetAllPeriodSelectList()
    {
        var periods = await _uow.Periods.GetAll();
        return periods.OrderByDescending(p => p.CreatedAt).Select(p => new SelectItemDTO { Id = p.Id, Name = p.Name });
    }
    public async Task<GetPeriodDTO> GetNewPeriod()
    {
        var DTO = new GetPeriodDTO();
        var LastPeriod = await _uow.Periods.GetLastOrderBy(p => p.Id);
        var Settings = await _uow.Settings.GetFirst();

        DTO.DaysCount = Settings?.DefaultPeriodDays ?? 7;
        DTO.From = LastPeriod != null ? LastPeriod.To.AddDays(1) : DateTime.Now;
        DTO.To = DTO.From.AddDays(DTO.DaysCount);

        return DTO;
    }
    public async Task<GetPeriodDTO> GetPeriodById(int id)
    {
        var period = await _uow.Periods.Get(p => p.Id == id, "Journals");

        if (period == null)
            return null;

        var journals = period.Journals.Select(j => new JournalListItemDTO
        {
            Code = j.Code,
            CreatedAt = j.CreatedAt,
            Detail = j.Detail,
            Type = j.Type,
            Amount = j.Amount,
            Notes = j.Notes,
        });

        var periodDto = new GetPeriodDTO
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
    public async Task<GetPeriodDTO> GetLastPeriod()
    {
        var lastPeriod = await _uow.Periods.GetLastOrderBy(p => p.Id ,"Journals");
        if (lastPeriod == null)
            return null;

        var journals = lastPeriod.Journals.Select(j => new JournalListItemDTO
        {
            Code = j.Code,
            CreatedAt = j.CreatedAt,
            Detail = j.Detail,
            Type = j.Type,
            Amount = j.Amount,
            Notes = j.Notes,
            
        });

        var lastPeriodDto = new GetPeriodDTO
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
    public async Task<ConfirmationResponse> CreatePeriod(CreatePeriodDTO DTO)
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
    public async Task<ConfirmationResponse> EditPeriod(CreatePeriodDTO DTO)
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
    private async Task<int> GetPeriodNumber(DateTime FromDate)
    {
        int periodsInSameMonth = await _uow.Periods.Count(p => p.From.Month <= FromDate.Month && p.From.Year <= FromDate.Year);
        return periodsInSameMonth == 0 ? 1 : periodsInSameMonth + 1;
    }
}
