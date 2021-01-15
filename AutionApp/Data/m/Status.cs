using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.Data.Models
{
    public class Status
    {
        public int StatusId { get; set; }
        public string Title { get; set; }

        public List<LotStatus> LotStatuses { get; set; }
    }
}
