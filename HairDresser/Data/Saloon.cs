using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HairDresser1.Data
{
    public class Saloon
    {
        public string ID { get; set; }
        public string SaloonName { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string SaloonOwnerID { get; set; }
        public string SaloonOwnerName { get; set; }
        public string SaloonAdress { get; set; }
        public byte[] Images { get; set; }
    }
}
