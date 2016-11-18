using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class PatientAllergies_Custom
    {

        //public long allergiesID { get; set; }
        public string allergyName { get; set; }
        public string severity { get; set; }
        public string reaction { get; set; }
        public Nullable<long> patientID { get; set; }
        public string status { get; set; }
        public string userId { get; set; }
        public DateTime reporteddate { get; set; }
    }
}
