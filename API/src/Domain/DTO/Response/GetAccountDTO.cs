using Domain.DTO.Request;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Response;
public class GetAccountDTO : CreateAccountDTO
{
    public string? AccountNumber { get; set; }
    public string? ParentNumber { get; set; }
    public string? ParentName { get; set; }
    public bool IsParent { get; set; }
    public bool IsArchive { get; set; }
    public IEnumerable<SelectItemDTO> Accounts { get; set; }
}
