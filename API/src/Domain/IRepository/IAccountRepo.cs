using Domain.DTO.Response;
using Domain.Entities;

namespace Domain.IRepository;

public interface IAccountRepo : IRepository<Account>
{
    Task<LastAccountJournal?> GetLastAccountJournal(int accountId, int? costcenterId);
}