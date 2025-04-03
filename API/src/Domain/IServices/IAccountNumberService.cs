using Domain.DTO.Request;
using Domain.DTO.Response;

namespace Domain.IServices;

public interface IAccountNumberService
{
    public Task<GetAccountNumberAndLevelResponse> GetAccountNumberAndLevel(CreateAccountDTO DTO);
}
