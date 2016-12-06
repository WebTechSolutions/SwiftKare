using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class GetDoctorLanguages
    {
        public long doctorLanguageID { get; set; }
        public long doctorID { get; set; }
        public string languageName { get; set; }
    }
}
