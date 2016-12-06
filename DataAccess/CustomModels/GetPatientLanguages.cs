using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class GetPatientLanguages
    {
        public long patientLanguageID { get; set; }
        public long patientID { get; set; }
        public string languageName { get; set; }
    }
}
