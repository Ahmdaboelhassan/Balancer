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
    public async Task<IEnumerable<PeriodListItemDTO>> Search(string criteria)
    {
        var periods = await _uow.Periods.GetAll(p => p.Name.Contains(criteria));

        return periods.OrderByDescending(p => p.To).Select( sp => new PeriodListItemDTO
        {
            Id = sp.Id,
            Name = sp.Name,
            From = sp.From.ToShortDateString(),
            To = sp.To.ToShortDateString(),
            TotalAmount = sp.TotalAmount,
            IncreasedBalance = sp.TotalAmount >= 0,

        });
    }
    public async Task<IEnumerable<PeriodListItemDTO>> GetAll(int page)
    {
        const int pageSize = 20;

        return await _uow.Periods.SelectSome(p => true,
          sp => new PeriodListItemDTO
          {
              Id = sp.Id,
              Name = sp.Name,
              From = sp.From.ToShortDateString(),
              To = sp.To.ToShortDateString(),
              TotalAmount = sp.TotalAmount,
              IncreasedBalance = sp.TotalAmount >= 0,

          }, p => p.To, page, pageSize);
    }
    public async Task<IEnumerable<PeriodListItemDTO>> GetAll(DateTime From, DateTime To)
    {
        var periods = await _uow.Periods.GetAll(p => p.From >= From.Date && p.From <= To.Date);  
        return periods.OrderByDescending(p => p.To).Select(sp => new PeriodListItemDTO
        {
            Id = sp.Id,
            Name = sp.Name,
            From = sp.From.ToShortDateString(),
            To = sp.To.ToShortDateString(),
            TotalAmount = sp.TotalAmount,
            IncreasedBalance = sp.TotalAmount >= 0,
        });
    }

    public async Task<IEnumerable<SelectItemDTO>> GetAllSelectList()
    {
        var periods = await _uow.Periods.GetAll();
        return periods.OrderByDescending(p => p.CreatedAt).Select(p => new SelectItemDTO { Id = p.Id, Name = p.Name });
    }
    public async Task<GetPeriodDTO?> New()
    {
        var DTO = new GetPeriodDTO();
        var lastPeriod = await _uow.Periods.GetLastOrderBy(p => p.Id);

        var Settings = await _uow.Settings.GetFirst();
        DTO.DaysCount = Settings?.DefaultPeriodDays ?? 7;
        DTO.From = lastPeriod != null ? lastPeriod.To.AddDays(1) : DateTime.Now;
        DTO.To = DTO.From.AddDays(DTO.DaysCount - 1);
        DTO.Notes = $"New Period In {DateTime.Now.ToShortTimeString()}";
        return DTO;
    }
    public async Task<GetPeriodDTO> GetById(int id)
    {
        var period = await _uow.Periods.Get(p => p.Id == id, "Journals");

        if (period == null)
            return new GetPeriodDTO();


        var periodDto = new GetPeriodDTO
        {
            CreatedAt = period.CreatedAt.ToString("F"),
            DaysCount = period.DaysCount,
            From = period.From,
            To = period.To,
            Id = period.Id,
            Name = period.Name,
            Notes = period.Notes,
            TotalAmount = period.TotalAmount,
            LastUpdatedAt = period.LastUpdatedAt?.ToString("F") ?? "",
        };

        return periodDto;
    }
    public async Task<GetPeriodDTO> GetLast()
    {
        var lastPeriod = await _uow.Periods.GetLastOrderBy(p => p.Id ,"Journals");
        if (lastPeriod == null)
            return new GetPeriodDTO();

        var lastPeriodDto = new GetPeriodDTO
        {
            CreatedAt = lastPeriod.CreatedAt.ToString("F"),
            DaysCount = lastPeriod.DaysCount,
            From = lastPeriod.From,
            To = lastPeriod.To,
            Id = lastPeriod.Id,
            Name = lastPeriod.Name,
            Notes = lastPeriod.Notes,
            TotalAmount = lastPeriod.TotalAmount,
            LastUpdatedAt = lastPeriod.LastUpdatedAt?.ToString("F") ?? "",
        };

        return lastPeriodDto;
    }
    public async Task<ConfirmationResponse> Create(CreatePeriodDTO DTO)
    {
        var From = DTO.From;
        var To = DTO.To;

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
    public async Task<ConfirmationResponse> Edit(CreatePeriodDTO DTO)
    {
        if (DTO.Id == 0)
            return await Create(DTO);

        var period = await _uow.Periods.Get(DTO.Id);
        if (period is null)
            return new ConfirmationResponse { IsSucceed = false, Message = "Period Is Not Exist!" };

        var From = DTO.From;
        int periodNo = await GetPeriodNumber(From);

        var periodName = $"Period No. {periodNo} In Month {From.Month} In Year {From.Year}";

        period.Name = periodName;
        period.From = DTO.From;
        period.To = DTO.To;
        period.LastUpdatedAt = DateTime.Now;
        period.DaysCount = DTO.DaysCount;
        period.Notes = DTO.Notes;

        _uow.Periods.Update(period);
        await _uow.SaveChangesAync();
        return new ConfirmationResponse { IsSucceed = true, Message = "Period Has Been Updated Successfully" };
    }
    public async Task<ConfirmationResponse> Delete(int id)
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
        int periodsInSameMonth = await _uow.Periods.Count(p => p.From.Month == FromDate.Month && p.From.Year == FromDate.Year);
        return periodsInSameMonth == 0 ? 1 : periodsInSameMonth + 1;
    }

    
}
