using System;
namespace DataAccess.CustomModels
{
    public class DoctorTimingsModel
    {
        public long doctorTimingsID { get; set; }
        public long doctorID { get; set; }
        public string day { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string username { get; set; }
    }
}
