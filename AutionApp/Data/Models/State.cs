using System;
using System.Collections.Generic;

namespace AutionApp
{
    public partial class State
    {
        public State()
        {
            StatesLots = new HashSet<StatesLots>();
        }

        public int StateId { get; set; }
        public string Title { get; set; }

        public virtual ICollection<StatesLots> StatesLots { get; set; }
    }
}
