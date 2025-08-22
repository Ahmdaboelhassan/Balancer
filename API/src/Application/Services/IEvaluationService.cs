using Domain.DTO.Response;

namespace Application.Services
{
    public interface IEvaluationService
    {
        Task<Result<EvaluationDTO>> CalculateDetailsBalances(EvaluationDTO DTO);
        Task<ConfirmationResponse> Create(EvaluationDTO DTO, int userId);
        Task<ConfirmationResponse> Delete(int id);
        Task<ConfirmationResponse> Edit(EvaluationDTO DTO);
        Task<IEnumerable<EvaluationListItemDTO>> GetAll(DateTime From, DateTime To);
        Task<EvaluationDTO> GetById(int id);
        Task<EvaluationDTO?> New();
    }
 }
