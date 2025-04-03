using Domain.DTO.Request;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Response;
public class GetJournalDTO : CreateJournalDTO
{
    public int Id { get; set; }
    public int Code { get; set; } // auto Generate
    public string Notes { get; set; } // auto Generate
    public string? LastUpdatedAt { get; set; } // auto Generate
    public byte Type { get; set; }
    [Required]
    public IEnumerable<SelectItemDTO> Accounts { get; set; }
    public IEnumerable<SelectItemDTO> CostCenters { get; set; }

}
