using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CommonModels
{
    public class FetchTimingsModel
    {
        public string appDate { get; set; }
        public long doctorID { get; set; }
    }
    public class TimingsVM
    {
        public TimeSpan? fromtime { get; set; }
        public TimeSpan? totime { get; set; }
        public long? doctorID { get; set; }
    }
    public class AppointmentsVM
    {
        public TimeSpan? appTime { get; set; }
       public long doctorID { get; set; }
    }
    public class DocTimingsAndAppointment
    {
        public List<TimingsVM> timingsVM { get; set; }
        public List<AppointmentsVM> appointmentVM { get; set; }
    } 
}
