using Domain.DTO.Response;

namespace Domain.IServices;
public interface IHomeService
{
    public Task<GetHomeDTO> GetHome();
}
