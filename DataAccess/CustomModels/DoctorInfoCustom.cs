using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class DoctorInfoCustom
    {
        public long doctorID { get; set; }
        public string doctorName { get; set; }
        public string gender { get; set; }
        
        public string email { get; set; }
        public string state { get; set; }
        public long consultCharges { get; set; }
        
        public string cellPhone { get; set; }
        public string specialityName { get; set; }
        public string languageName { get; set; }
        public byte[] picture { get; set; }



    }
    
}
