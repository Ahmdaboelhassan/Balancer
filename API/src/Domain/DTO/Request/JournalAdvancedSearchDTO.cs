using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Request
{
    public class JournalAdvancedSearchDTO
    {
        public string? key { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int orderBy { get; set; }
        public int type { get; set; }
        public bool filterByDate { get; set; }
        public bool filterByKey { get; set; }
    }
}
