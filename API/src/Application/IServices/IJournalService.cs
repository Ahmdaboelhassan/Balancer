﻿
using Application.DTO.Request;
using Application.DTO.Response;
using Application.Models;

namespace Application.IServices;
public interface IJournalService
{
    public Task<GetJournalDTO> New(int? periodId);
    public Task<GetJournalDTO> Get(int id);
    public Task<IEnumerable<JournalListItemDTO>> GetAll(int page);
    public Task<PeriodJournals> GetPeriodJournals(int periodId);
    public Task<IEnumerable<JournalListItemDTO>> GetAll(DateTime from , DateTime to);
    public Task<IEnumerable<JournalListItemDTO>> Search(string criteria);
    public Task<int> GetNextCode();
    public Task<ConfirmationResponse> Create(CreateJournalDTO model);
    public Task<ConfirmationResponse> Edit(CreateJournalDTO model);
    public Task<ConfirmationResponse> Delete(int id);

}
