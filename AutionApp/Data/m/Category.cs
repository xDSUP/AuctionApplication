using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.Data.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [MaxLength(128)]
        public string Title { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        public Category Parent { get; set; }
        [InverseProperty("Parent")]
        public virtual ICollection<Category> ParentChildren { get; set; }

        public List<Lot> Lots { get; set; }
    }
}
