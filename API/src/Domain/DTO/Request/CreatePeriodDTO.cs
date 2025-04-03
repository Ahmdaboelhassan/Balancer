using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Request;
public class CreatePeriodDTO
{
    public int Id { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int DaysCount { get; set; }
    public string? Notes { get; set; }
}
