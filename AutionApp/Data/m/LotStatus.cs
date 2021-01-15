using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.Data.Models
{
    public class LotStatus
    {
        public Lot Lot { get; set; }
        public Status Status { get; set; }
        public DateTime DateTime { get; set; }
    }
}
