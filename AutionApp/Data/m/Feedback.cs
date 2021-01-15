using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.Data.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        /// <summary> Автор отзыва </summary>
        public User Author { get; set; }
        /// <summary> На кого отзыв </summary>
        public User User { get; set; }
        /// <summary> Оценка </summary>
        public int Mark { get; set; }
        /// <summary> На кого отзыв </summary>
        [MaxLength(512)]
        public string Desc { get; set; }
 
    }
}
