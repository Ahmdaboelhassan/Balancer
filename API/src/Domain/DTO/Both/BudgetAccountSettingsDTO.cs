namespace Domain.DTO.Both
{
    public class BudgetAccountSettingsDTO
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Budget { get; set; }
        public string Color { get; set; }
        public string DisplayName { get; set; }
    }
}
