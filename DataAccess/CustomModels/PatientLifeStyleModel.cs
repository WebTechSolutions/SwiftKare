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

    public class GetPatientLifeStyle

    {
        public long patientlifestyleID { get; set; }
        public long patientID { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
    }

    public class GetLifeStyleQuestions

    {
        public long questionID { get; set; }
        public string question { get; set; }
    }

    public class PLifeStyleList
    {
        public Nullable<long> questionID { get; set; }
        public long patientLifeStyleID { get; set; }
        public Nullable<long> patientID { get; set; }
        public string question { get; set; }
        public string answer { get; set; }

    }
}
