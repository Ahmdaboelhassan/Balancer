using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Request;

public class AdjustBudgetRequest
{
    public decimal Amount { get; set; }
    public bool Increase { get; set; }
}
