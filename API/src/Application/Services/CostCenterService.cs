using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.IRepository;
using Domain.IServices;
using Domain.Entities;


namespace Application.Services;
public class CostCenterService : ICostCenterService
{
    private readonly IUnitOfWork _uow;
    public CostCenterService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<GetCostCenter>> GetAll()
    {
        return await _uow.CostCenter.SelectAll(a => true, a => new GetCostCenter
        {
            Id = a.Id,
            CreatedAt = a.CreatedAt.ToShortDateString(),
            Description = a.Description,
            Name = a.Name,
            IsArchived = a.IsArchived,
        });
    }

    public async Task<IEnumerable<SelectItemDTO>> GetAllSelectList()
    {
        return (await _uow.CostCenter.SelectAll(a => !a.IsArchived, a => new SelectItemDTO
        {
            Id = a.Id,
            Name = a.Name,
        })).OrderBy(c => c.Name);
    }

    public async Task<GetCostCenter> Get(int id)
    {
        var costCenter = await _uow.CostCenter.Get(id);

        if (costCenter is null)
            return new GetCostCenter { Name = "Invalid Cost Center Id" };

        return new GetCostCenter
        {
            Id = costCenter.Id,
            CreatedAt = costCenter.CreatedAt.ToShortDateString(),
            Description = costCenter.Description,
            Name = costCenter.Name,
            IsArchived = costCenter.IsArchived,
        };

    }
    public async Task<ConfirmationResponse> Create(CreateCostCenter model)
    {
        if (string.IsNullOrEmpty(model.Name))
            return new ConfirmationResponse { Message = "Invalid Cost Center Name" };

        var newCostCenter = new CostCenter
        {
            Name = model.Name,
            CreatedAt = DateTime.Now,
            Description = model.Description,
            IsArchived = model.IsArchived,
        };

        await _uow.CostCenter.AddAsync(newCostCenter);
        await _uow.SaveChangesAync();
        return new ConfirmationResponse { Message = "Cost Center Has Been Created Successfully"  , IsSucceed = true};
    }

    public async Task<ConfirmationResponse> Edit(CreateCostCenter model)
    {
        if (model.Id == 0)
            return await Create(model);

        var costCenter = await _uow.CostCenter.Get(model.Id);
        if (costCenter is null)
            return new ConfirmationResponse { Message = "Invalid Cost Center" };


        if (string.IsNullOrEmpty(model.Name))
            return new ConfirmationResponse { Message = "Invalid Cost Center Name" };



        costCenter.Name = model.Name;
        costCenter.CreatedAt = DateTime.Now;
        costCenter.Description = model.Description;
        costCenter.IsArchived = model.IsArchived;

        _uow.CostCenter.Update(costCenter);
        await _uow.SaveChangesAync();
        return new ConfirmationResponse { Message = "Cost Center Has Been Updated Successfully", IsSucceed = true };
    }

    public async Task<ConfirmationResponse> Delete(int id)
    {
    
        var costCenter = await _uow.CostCenter.Get(id);
        if (costCenter is null)
            return new ConfirmationResponse { Message = "Invalid Cost Center" };

        if (await _uow.JournalDetail.Exists(d => d.CostCenterId == id))
            return new ConfirmationResponse { Message = "Can't Delete Cost Center It Has Journal" };

        costCenter.IsDeleted = true;

        _uow.CostCenter.Update(costCenter);
        await _uow.SaveChangesAync();
        return new ConfirmationResponse { Message = "Cost Center Has Been Deleted Successfully", IsSucceed = true };
    }


}
