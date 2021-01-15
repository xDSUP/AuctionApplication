using System;
using System.Collections.Generic;

namespace AutionApp
{
    public partial class StatesLots
    {
        public int LotId { get; set; }
        public int StateId { get; set; }
        public DateTime Time { get; set; }

        public virtual Lot Lot { get; set; }
        public virtual State State { get; set; }
    }
}
