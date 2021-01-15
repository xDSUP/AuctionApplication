using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.Data.Models
{
    public class User : IdentityUser
    {
        public byte[] Avatar { get; set; }

        public List<Lot> Lots { get; set; }
        public List<Feedback> AuthorFeedbacks { get; set; }
        public List<Feedback> Feedbacks { get; set; }
        public List<Bet> Bets { get; set; }
    }
}
