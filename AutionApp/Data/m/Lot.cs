using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.Data.Models
{
    public class Lot
    {
        public int LotId { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal MinPrice { get; set; }
        public decimal Step { get; set; }
        public Category Category { get; set; }

        public Status Status { get; set; }

        public List<Bet> Bets { get; set; }
        public List<LotStatus> LotStatuses { get; set; }
    }
}
