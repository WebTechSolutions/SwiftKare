using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestAPIs.Models
{
    public class SeeDoctorDTO
    {
        public string firstName { get; set; }
        public long id { get; set; }
        public string lastName { get; set; }
        public string gender { get; set; }
        public virtual ICollection<DoctorLanguage> DoctorLanguages { get; set; }
        public virtual ICollection<DoctorSpeciality> DoctorSpecialities { get; set; }
        public virtual ICollection<DoctorTiming> DoctorTimings { get; set; }
    }
}