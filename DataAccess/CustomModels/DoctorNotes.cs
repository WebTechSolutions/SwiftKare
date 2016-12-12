using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class DoctorNotes
    {
        public long consutID { get; set; }
        public string subjective { get; set; }
        public string objective { get; set; }
        public string assessment { get; set; }
        public string plans { get; set; }


        public string userEmail { get; set; }
    }
}
