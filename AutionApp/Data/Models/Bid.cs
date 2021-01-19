using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutionApp
{
    public partial class Bid
    {
        public int BidId { get; set; }
        [Display(Name = "Время ставки")]
        public DateTime Time { get; set; }
        [Column("UserId")]
        [Display(Name = "Пользователь")]
        public string UserId { get; set; }
        [Display(Name = "Лот")]
        public int LotId { get; set; }
        [Display(Name = "Ставка")]
        [Required]
        public decimal Rate { get; set; }

        public virtual Lot Lot { get; set; }
        public virtual User User { get; set; }

    }
}
