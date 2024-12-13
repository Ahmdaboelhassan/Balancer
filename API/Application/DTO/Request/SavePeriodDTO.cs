using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Request;
public class SavePeriodDTO
{
    public int Id { get; set; }
    public DateOnly From { get; set; }
    public int DaysCount { get; set; }
    public string? Notes { get; set; }
}
