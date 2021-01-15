using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutionApp
{
    public partial class Lot
    {
        public Lot()
        {
            Bids = new HashSet<Bid>();
            StatesLots = new HashSet<StatesLots>();
        }

        public int LotId { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public decimal StartPrice { get; set; }
        public decimal Step { get; set; }
        public int? CategoryId { get; set; }
        [Column("UserId")]
        public string UserId { get; set; }

        public virtual Category Category { get; set; }
        //[ForeignKey("Id")]
        public virtual User User { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<StatesLots> StatesLots { get; set; }
    }
}
