using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DTO.Response
{
    public class EvaluationDTO
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string? Name { get; set; }
        public decimal Profit { get; set; }
        public decimal? ProfitPercentage { get; set; }
        public decimal Income { get; set; }
        public string? Note { get; set; }
        public string?  LastUpdatedAt { get; set; }
        public string? CreatedAt { get; set; }
        public IEnumerable<EvaluationDetailDTO> EvaluationDetails { get; set; }
    }

    public class EvaluationDetailDTO
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Percentage { get; set; }
    }
}
