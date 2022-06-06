using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HairDresser1.Models
{
    public class HairDresserModel
    {
        public string ID { get; set; }
        public string SaloonID { get; set; }
        public string HairdresserName { get; set; }
        public string HairdresserSurname { get; set; }
        public int HairdresserPhoneNumber { get; set; }
        public string HairdresserEmail { get; set; }
        public string HairdresserDescription { get; set; }
        public IFormFile Image { get; set; }
    }
}
