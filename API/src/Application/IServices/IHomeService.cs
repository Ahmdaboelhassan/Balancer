using Application.DTO.Response;

namespace Application.IServices;
public interface IHomeService
{
    public Task<GetHomeDTO> GetHome();
}
