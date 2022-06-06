using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HairDresser1.Data
{
    public class Appointment
    {
        public string ID { get; set; }
        public string SaloonID { get; set; }
        public string HairDresserID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string UserID { get; set; }        
    }
}
