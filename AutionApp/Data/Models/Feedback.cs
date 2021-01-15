using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutionApp
{
    public partial class Feedback
    {
        public int FeedBackId { get; set; }
        [Column("AuthorId")]
        public string AuthorId { get; set; }
        [Column("UserId")]
        public string UserId { get; set; }
        public DateTime Time { get; set; }
        public string Desc { get; set; }
        public int Mark { get; set; }
        [ForeignKey("AuthorId"), ]
        public virtual User Author { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
