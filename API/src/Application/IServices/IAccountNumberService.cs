using Application.DTO.Request;
using Application.Models;

namespace Application.IServices;

public interface IAccountNumberService
{
    public Task<GetAccountNumberAndLevelResponse> GetAccountNumberAndLevel(CreateAccountDTO DTO);
}
