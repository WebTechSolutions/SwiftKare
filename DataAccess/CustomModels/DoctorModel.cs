using System;
namespace DataAccess.CustomModels
{
    public class DoctorModel
    {
        public long doctorID { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string email { get; set; }
        public string userId { get; set; }
        public string secretQuestion1 { get; set; }
        public string secretQuestion2 { get; set; }
        public string secretQuestion3 { get; set; }

        public string secretAnswer1 { get; set; }
        public string secretAnswer2 { get; set; }
        public string secretAnswer3 { get; set; }

        public Nullable<bool> active { get; set; }
        public Nullable<bool> status { get; set; }
    }
    public class DoctorDataset
    {
        public long doctorID { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public byte[] picture { get; set; }
    }
}
