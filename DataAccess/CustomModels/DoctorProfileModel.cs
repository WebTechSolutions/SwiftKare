using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
   public class DoctorProfileModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string title { get; set; }
        public string suffix { get; set; }
        public string city { get; set; }
        public string aboutMe { get; set; }
        public string specialization { get; set; }
        public string workExperience { get; set; }
        public string education { get; set; }
        public string publication { get; set; }
        public Nullable<long> consultCharges { get; set; }
        public string state { get; set; }
        public string timezone { get; set; }
        public List<DoctorLicenseState> licensedState { get; set; }
        public string gender { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string homePhone { get; set; }
        public string cellPhone { get; set; }
        public string zip { get; set; }
        public Nullable<System.DateTime> dob { get; set; }
        public byte[] picture { get; set; }
        public string ProfileBase64 { get; set; }

    }
}
