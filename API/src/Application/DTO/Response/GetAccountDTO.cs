
using Application.DTO.Request;
using Application.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Response;
public class GetAccountDTO : CreateAccountDTO
{
    public string? AccountNumber { get; set; }
    public string? ParentNumber { get; set; }
    public string? ParentName { get; set; }
    public bool IsParent { get; set; }
    public IEnumerable<SelectItemDTO> Accounts { get; set; }
}
