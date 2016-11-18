using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CommonModels
{
    public class SearchDoctorModel
    {
        public string language { get; set; }
        public string speciality { get; set; }
        public string name { get; set; }
        public DateTime appDate { get; set; }
        public TimeSpan appTime { get; set; }
        public string gender { get; set; }
    }
}
