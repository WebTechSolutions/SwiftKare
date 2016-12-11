using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class CreateConsultModel
    {
        public long doctorID { get; set;}
        public long patientID { get; set; }
        public string sessionID { get; set; }
        public string token { get; set; }
    }
}
