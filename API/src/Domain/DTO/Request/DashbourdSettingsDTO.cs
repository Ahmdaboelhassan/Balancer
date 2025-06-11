using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Request
{
    public class DashboardSettingsDTO
    {
        public int Id { get; set; }
        [Required]
        public int Account1 { get; set; }
        [Required]
        public int Account2 { get; set; }
        [Required]
        public int Account3 { get; set; }
        [Required]
        public int Account4 { get; set; }
        public decimal OtherExpensesTarget { get; set; } 
        public bool ApplyOverBudgetToFunds { get; set; }
    }
}
