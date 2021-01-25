using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.ViewModels
{
    public class IndexViewModel
    {
        public User User { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
