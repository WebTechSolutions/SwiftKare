using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
   public  class GetMedication
    {
        public long medicationID { get; set; }
        public string medicineName { get; set; }
        public string frequency { get; set; }
        public Nullable<long> patientId { get; set; }
        public Nullable<DateTime> reporteddate { get; set; }
    }
}
