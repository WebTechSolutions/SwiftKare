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
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string cellPhone { get; set; }
        public string specialityName { get; set; }
       
        
    }
}
