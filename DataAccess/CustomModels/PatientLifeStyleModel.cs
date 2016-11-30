using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
   public  class PatientLifeStyleModel
    {
        public long patientlifestyleID { get; set; }
        public long patientID { get; set; }
        public string answer { get; set; }
    }
}
