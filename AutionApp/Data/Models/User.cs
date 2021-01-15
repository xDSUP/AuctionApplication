using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AutionApp
{
    public partial class User : IdentityUser
    {
        public User()
        {
            Bids = new HashSet<Bid>();
            Lots = new HashSet<Lot>();
        }

        public byte[] Avatar { get; set; }
        
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<Lot> Lots { get; set; }
    }
}
