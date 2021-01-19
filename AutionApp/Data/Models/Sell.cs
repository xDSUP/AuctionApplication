using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp
{
    public class Sell
    {
        public string UserId { get; set; }
        public int LotId { get; set; }

        public virtual User User { get; set; }
        public virtual Lot Lot { get; set; }
    }
}
