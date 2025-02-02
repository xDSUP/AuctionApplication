﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutionApp
{
    public partial class User : IdentityUser
    {
        public User()
        {
            Bids = new HashSet<Bid>();
            Lots = new HashSet<Lot>();
            Sells = new HashSet<Sell>();
        }
        
        [Column(TypeName = "image")]
        [Display(Name = "Аватарка")]
        public byte[] Avatar { get; set; }
        
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<Lot> Lots { get; set; }
        public virtual ICollection<Sell> Sells { get; set; }
    }
}
