using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
   public class GetPatientUserFiles
    {
        public long fileID { get; set; }
        public string documentType { get; set; }
        public string FileName { get; set; }
        public byte[] fileContent { get; set; }
        public Nullable<long> patientID { get; set; }
        public Nullable<long> doctorID { get; set; }
    }
}
