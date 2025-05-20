using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Response
{
    public class AccountComparerDTO
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Label { get; set; }
        public string GroupType { get; set; }
        public IEnumerable<decimal> Amounts { get; set; }
        public IEnumerable<string> Time { get; set; }
    }
}
