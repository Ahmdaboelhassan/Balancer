
using Application.DTO.Request;
using Application.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Response;
public class GetJournalDTO : CreateJournalDTO
{
    public int Code { get; set; } // auto Generate
    public string Notes { get; set; } // auto Generate
    public string PeriodName { get; set; }
    public string CostCenterName { get; set; }
    public DateTime CreatedAt { get; set; } // auto Generate
    public DateTime? LastUpdatedAt { get; set; } // auto Generate
    public byte Type { get; set; }
    [Required]
    public IEnumerable<SelectItemDTO> Accounts { get; set; }
    public IEnumerable<SelectItemDTO> CostCenters { get; set; }

}
