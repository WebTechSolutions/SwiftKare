using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class PatientProfileModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string title { get; set; }
        public string suffix { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string timezone { get; set; }
        public Nullable<long> height { get; set; }
        public Nullable<long> weight { get; set; }
        public string email { get; set; }
        public string secretQuestion1 { get; set; }
        public string secretQuestion2 { get; set; }
        public string secretQuestion3 { get; set; }
        public string secretAnswer1 { get; set; }
        public string secretAnswer2 { get; set; }
        public string secretAnswer3 { get; set; }
        public string gender { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string homePhone { get; set; }
        public string cellPhone { get; set; }
        public string zip { get; set; }
        public Nullable<System.DateTime> dob { get; set; }
        public byte[] picture { get; set; }
    }
}
