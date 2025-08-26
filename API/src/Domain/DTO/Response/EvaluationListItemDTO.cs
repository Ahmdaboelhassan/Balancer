namespace Domain.DTO.Response
{
    public class EvaluationListItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public bool IncreasedProfit { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitPercentage { get; set; }
        
    }
}
