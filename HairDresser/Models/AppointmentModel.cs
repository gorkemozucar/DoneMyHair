using HairDresser1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HairDresser1.Models
{
    public class AppointmentModel
    {
        public string ID { get; set; }
        public Saloon Saloon { get; set; }
        public HairDresser HairDresser { get; set; }
        public DateTime AppointmentDate { get; set; }
        public ApplicationUser User { get; set; }
    }
}
