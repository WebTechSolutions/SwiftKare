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
        public string appDate{ get; set; }
        public Nullable<TimeSpan> appTime { get; set; }
        public string gender { get; set; }
        public long patientID { get; set; }
    }
    public class FavDoctors
    {
        public long doctorID { get; set; }
    }


    public class SearchDoctorResult
    {
        List<SearchDoctorModel> _objSearchDoctorModel { get; set; }
        List<FavDoctors> _objFavDoctors { get; set; }
    }
   
}
