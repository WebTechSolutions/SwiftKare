using DataAccess;
using System.Collections.Generic;
using System.Web.Mvc;
using DataAccess.CustomModels;

namespace WebApp.Models
{
    public class DoctorTimingsViewModel
    {
        public List<DoctorTimingsModel> DoctorTimingsList { get; set; }
        public DoctorTimingsModel DoctorTiming { get; set; }
        public long DoctorId { get; set; }
        public List<DoctorTimingsListModel> DayWiseTimings { get; set; }
    }
    public class DoctorTimingsListModel
    {
        public string Day { get; set; }
        public List<DoctorTimingsModel> Timings { get; set; }
    }

}
