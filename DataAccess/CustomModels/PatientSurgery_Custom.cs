using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class PatientSurgery_Custom
    {
        //public long surgeryID { get; set; }
        public Nullable<long> patientID { get; set; }
        public string bodyPart { get; set; }
     }
}
