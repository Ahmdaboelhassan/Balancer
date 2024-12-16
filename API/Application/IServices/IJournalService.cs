
using Application.DTO.Request;
using Application.DTO.Response;
using Application.Models;

namespace Application.IServices;
public interface IJournalService
{
    public Task<GetJournalDTO> NewJournal(int periodId);
    public Task<GetJournalDTO> GetJournal(int id);
    public Task<IEnumerable<JournalListItemDTO>> GetAllJournal(DateTime? from , DateTime? to);
    public Task<IEnumerable<JournalListItemDTO>> GetAllJournal(int periodId);
    public Task<int> GetNextCode();
    public Task<IEnumerable<JournalDetailDTO>> GetAccountStatement(int? AccountId , DateTime? from , DateTime? to);
    public Task<ConfirmationResponse> CreateJournal(CreateJournalDTO model);
    public Task<ConfirmationResponse> EditJournal(CreateJournalDTO model);
    public Task<ConfirmationResponse> DeleteJournal(int id);

}
