using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutionApp
{
    public partial class Bid
    {
        public int BidId { get; set; }
        public DateTime Time { get; set; }
        [Column("UserId")]
        public string UserId { get; set; }
        public int LotId { get; set; }
        public decimal Rate { get; set; }

        public virtual Lot Lot { get; set; }
        public virtual User User { get; set; }
    }
}
