using DataAccess;
using System.Collections.Generic;
using System.Web.Mvc;
using DataAccess.CommonModels;

namespace WebApp.Models
{
    public class DoctorTimingsViewModel
    {
        public List<DoctorTimingsModel> DoctorTimingsList { get; set; }
        public DoctorTimingsModel DoctorTiming { get; set; }
        public long DoctorId { get; set; }
    }
}
