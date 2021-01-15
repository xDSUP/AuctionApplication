using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.Data.Models
{
    public class Bet
    {
        public int BetId { get; set; }
        public DateTime DateTime { get; set; }
        public User User { get; set; }
        public Lot Lot { get; set; }
        public decimal Price { get; set; }
    }
}
