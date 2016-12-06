using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
   public  class GetRescheduleAppModel
    {
        public long doctorID { get; set; }
        public long patientID { get; set; }
    }
}
