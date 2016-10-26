using DataAccess;
using System.Collections.Generic;
using System.Web.Mvc;

namespace WebApp.Models
{
    public class DoctorTimingsViewModel
    {
        public IEnumerable<DoctorTiming> DoctorTimingsList { get; set; }
        public DoctorTiming DoctorTiming { get; set; }
        public Timing Timing { get; set; } = new Timing();
        public IEnumerable<SelectListItem> Days { get; set; }
    }

    public class Timing
    {
        public string From { get; set; } = "";
        public string To { get; set; } = "";
    }

}
