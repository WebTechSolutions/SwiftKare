using DataAccess.CustomModels;
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
        public string appDate { get; set; }
        public string appTime { get; set; }
        public string gender { get; set; }
        public long patientID { get; set; }
    }
    
    public class SearchDoctorResult
    {
        public List<SearchDoctorWithShift_Result> doctor { get; set; }
        public List<FavouriteDoctorModel> favdoctor { get; set; }
    }
   
}
