using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class RescheduleAppModel
    {
        public long doctorID { get; set;}
        public long patientID { get; set; }
        public string docName { get; set;}
        public string patName { get; set; }
        public Nullable<DateTime> appDate { get; set; }
        public Nullable<TimeSpan> appTime { get; set; }
        public string rov { get; set; }
        public string chiefComplaints { get; set; }
    }
}
