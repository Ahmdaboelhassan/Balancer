namespace Domain.DTO.Response
{
    public record BudgetAccountDTO
    {
        public string DisplayName { get; set; }
        public decimal Budget { get; set; }
        public decimal Spent{ get; set; }
        public decimal Percentage { get; set; }
        public string Color { get; set; }
    }
}
