using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class RescheduleAppModel
    {
        public string consultationStatus { get; set; }
        public long appID { get; set; }

        public long doctorID { get; set;}
        public long patientID { get; set; }
        public string DoctorName { get; set;}
        public string PatientName { get; set; }
        public string appDate { get; set; }
        public string appTime { get; set; }
        public string rov { get; set; }
        public string chiefComplaints { get; set; }

        
       

    }

   


}
