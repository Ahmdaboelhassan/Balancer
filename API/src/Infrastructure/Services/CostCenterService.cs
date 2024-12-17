
using Application.DTO.Request;
using Application.DTO.Response;
using Application.IRepository;
using Application.IServices;
using Application.Models;
using Domain.Models;


namespace Infrastructure.Services;
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
            CreatedAt = a.CreatedAt,
            Description = a.Description,
            Name = a.Name,
        });
    }

    public async Task<GetCostCenter> Get(int id)
    {
        var costCenter = await _uow.CostCenter.Get(id);

        if (costCenter is null)
            return new GetCostCenter { Name = "Invalid Cost Center Id" };

        return new GetCostCenter
        {
            Id = costCenter.Id,
            CreatedAt = costCenter.CreatedAt,
            Description = costCenter.Description,
            Name = costCenter.Name,
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

        _uow.CostCenter.Update(costCenter);
        await _uow.SaveChangesAync();
        return new ConfirmationResponse { Message = "Cost Center Has Been Updated Successfully", IsSucceed = true };
    }

    public async Task<ConfirmationResponse> Delete(int id)
    {
    
        var costCenter = await _uow.CostCenter.Get(id);
        if (costCenter is null)
            return new ConfirmationResponse { Message = "Invalid Cost Center" };

        costCenter.IsDeleted = true;

        _uow.CostCenter.Update(costCenter);
        await _uow.SaveChangesAync();
        return new ConfirmationResponse { Message = "Cost Center Has Been Deleted Successfully", IsSucceed = true };
    }


}
