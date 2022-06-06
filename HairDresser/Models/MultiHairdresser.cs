using HairDresser1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HairDresser1.Models
{
    public class MultiHairdresser
    {
        public HairDresser hairDresserModel {get; set;}
        public List<CommentModel> commentsModel { get; set; }
        public CommentModel commentModel { get; set; }
        public Appointment appointment { get; set; }
    }
}
