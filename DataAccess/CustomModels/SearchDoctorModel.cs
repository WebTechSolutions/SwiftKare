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
    public class SearchDoctorWithShift_Model
    {
        public string ProfilePhotoBase64 { get; set; }
        public long doctorID { get; set; }
        public string title { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string city { get; set; }
        public byte[] picture { get; set; }
        public string state { get; set; }
        public string languageName { get; set; }
        public string specialityName { get; set; }
        public int? reviewStar { get; set; }
    }
    public class SearchDoctorResult
    {
        public List<SearchDoctorWithShift_Model> doctor { get; set; }
        public List<FavouriteDoctorModel> favdoctor { get; set; }
    }
   
}
