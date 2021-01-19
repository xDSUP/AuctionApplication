using System;
using System.Collections.Generic;

namespace AutionApp
{
    public partial class Category
    {
        public Category()
        {
            Childs = new HashSet<Category>();
            Lots = new HashSet<Lot>();
        }

        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public int? ParentId { get; set; }
        public bool IsGroup { get; set; }

        public virtual Category Parent { get; set; }
        public virtual ICollection<Category> Childs { get; set; }
        public virtual ICollection<Lot> Lots { get; set; }
    }
}
