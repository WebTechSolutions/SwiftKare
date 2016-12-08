using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class ViewPatientProfile
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string title { get; set; }
        public string suffix { get; set; }
        public int age { get; set; }
        public List<PatientLanguage> patlang { get; set; }
        public List<Condition> patcond { get; set; }
        public List<Medication> patmedication { get; set; }
        public List<PatientAllergy> patallergy { get; set; }
        public List<PatientSurgery> patsurgery { get; set; }
        public List<PatientFamilyHX> patfamilyhx { get; set; }
        public string gender { get; set; }
        public string cellPhone { get; set; }
        public string zip { get; set; }
        public Nullable<System.DateTime> dob { get; set; }
        public byte[] picture { get; set; }
    }
}
