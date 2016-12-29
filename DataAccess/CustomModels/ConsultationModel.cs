using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class ConsultationModel
    {
        public long consultID { get; set; }
        public string subjective { get; set; }
        public string objective { get; set; }
        public string assessment { get; set; }
        public string plans { get; set; }
        public List<ROSVM> rosItems { get; set; }
        public DoctorVM DoctorVM { get; set; }
        public PatientVM PatientVM { get; set; }
        public AppointmentVM AppointmentVM { get; set; }

    }
    public class ROSVM
    {
        public string systemItemName { get; set; }
    }
    public class AppointmentVM
    {
        public long appID { get; set; }
        public string appDate { get; set; }
        public string appTime { get; set; }
        public string rov { get; set; }
        public string chiefComplaints { get; set; }
        public int? paymentAmt { get; set; }
    }
}
