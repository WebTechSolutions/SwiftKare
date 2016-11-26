using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class PatientLifeStyle_Custom
    {
        public Nullable<long> patientID { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
    }
}
