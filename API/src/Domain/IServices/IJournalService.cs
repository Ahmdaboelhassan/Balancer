using Domain.DTO.Request;
using Domain.DTO.Response;

namespace Domain.IServices;
public interface IJournalService
{
    public Task<Result<GetJournalDTO>> New(int? periodId);
    public Task<Result<GetJournalDTO>> Get(int id);
    public Task<IEnumerable<JournalListItemDTO>> GetAll(int page);
    public Task<PeriodJournals> GetPeriodJournals(int periodId);
    public Task<IEnumerable<JournalListItemDTO>> GetAll(DateTime from , DateTime to);
    public Task<IEnumerable<JournalListItemDTO>> Search(string criteria);
    public Task<IEnumerable<JournalListItemDTO>> AdvancedSearch(JournalAdvancedSearchDTO DTO);
    public Task<int> GetNextCode();
    public Task<ConfirmationResponse> Create(CreateJournalDTO model);
    public Task<ConfirmationResponse> Edit(CreateJournalDTO model);
    public Task<ConfirmationResponse> Delete(int id);
}
