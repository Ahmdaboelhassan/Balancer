using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Response
{
    public record AccountComparerItemDTO
    {
        public string Time { get; set; }
        public decimal Amount { get; set; }
    }
}
