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
        public string cb { get; set; }
        public string mb { get; set; }
        public Nullable<System.DateTime> cd { get; set; }
        public Nullable<System.DateTime> md { get; set; }
        public Nullable<bool> active { get; set; }
    }
}
