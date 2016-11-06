using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class PatientROV_Custom
    {
        public long appId { get; set; }
        public string rov { get; set; }
        public string chiefcomplaints { get; set; }
        public long patientId { get; set; }
    }
}
