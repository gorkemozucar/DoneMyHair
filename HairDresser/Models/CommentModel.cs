using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HairDresser1.Models
{
    public class CommentModel
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public DateTime Time { get; set; }

        [Display(Name = "Add Comment")]
        public string Content { get; set; }
        public string HairDresserID { get; set; }
    }
}
