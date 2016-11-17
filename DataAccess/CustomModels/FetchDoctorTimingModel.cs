using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAccess.CommonModels
{
    public class FetchDoctorTimingModel
    {
        public Nullable<System.DateTime> appDate { get; set; }
        public Nullable<System.TimeSpan>  appTime { get; set; }
        public long doctorID { get; set; }
        public long patientID { get; set; }
        public List<string> mylist { get; set; }
        public Nullable<System.TimeSpan> from { get; set; }
        public Nullable<System.TimeSpan> to { get; set; }
        public Nullable<System.TimeSpan> docTime { get; set; }

    }
}