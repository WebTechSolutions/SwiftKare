using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestAPIs.Models
{
    public class BookAppointment
    {
        public DateTime appDate { get; set; }
        public TimeSpan appTime { get; set; }
        public long doctorID { get; set; }
        public long patientID { get; set; }
        public List<string> mylist { get; set; }
        public TimeSpan fromTime { get; set; }
        public TimeSpan toTime { get; set; }
        public TimeSpan docTime { get; set; }

    }
}